using System.Net;
using DeveloperProblemVault.Api.Config;

namespace DeveloperProblemVault.Api.Helpers;

public class KeycloakHelper(IHttpClientFactory httpClientFactory, KeycloakConfig config, ILogger<KeycloakHelper> logger)
{
    private readonly HttpClient _http = httpClientFactory.CreateClient();

    private string TokenUrl      => $"{config.AuthServerUrl}/realms/{config.Realm}/protocol/openid-connect/token";
    private string AdminTokenUrl => $"{config.AuthServerUrl}/realms/master/protocol/openid-connect/token";
    private string UsersUrl      => $"{config.AuthServerUrl}/admin/realms/{config.Realm}/users";

    public async Task<Dictionary<string, object>?> GetTokenAsync(string email, string password)
    {
        logger.LogInformation("Sending token request to Keycloak for {Email}", email);

        var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"]    = "password",
            ["client_id"]     = config.ClientId,
            ["client_secret"] = config.ClientSecret,
            ["username"]      = email,
            ["password"]      = password
        });

        var response = await _http.PostAsync(TokenUrl, form);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            logger.LogError("Keycloak token request failed for {Email} - {StatusCode} - {Error}", email, response.StatusCode, error);
            return null;
        }

        logger.LogInformation("Keycloak token request succeeded for {Email}", email);
        return await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
    }

    public async Task<string?> GetAdminTokenAsync()
    {
        logger.LogInformation("Requesting admin token from Keycloak master realm");

        var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"]    = "client_credentials",
            ["client_id"]     = config.AdminClientId,
            ["client_secret"] = config.AdminClientSecret
        });

        var response = await _http.PostAsync(AdminTokenUrl, form);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            logger.LogError("Keycloak admin token request failed - {StatusCode} - {Error}", response.StatusCode, error);
            return null;
        }

        logger.LogInformation("Admin token received successfully");
        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        return body?["access_token"]?.ToString();
    }

    public async Task<HttpStatusCode> CreateUserAsync(string adminToken, object user)
    {
        logger.LogInformation("Sending create user request to Keycloak Admin API");

        var request = new HttpRequestMessage(HttpMethod.Post, UsersUrl);
        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);
        request.Content = JsonContent.Create(user);

        var response = await _http.SendAsync(request);

        logger.LogInformation("Keycloak create user response - {StatusCode}", response.StatusCode);
        return response.StatusCode;
    }
}
