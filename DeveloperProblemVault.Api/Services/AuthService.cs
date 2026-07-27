using DeveloperProblemVault.Api.DTOs.Auth;
using DeveloperProblemVault.Api.Helpers;

namespace DeveloperProblemVault.Api.Services;

public class AuthService(IKeycloakHelper keycloakHelper, ILogger<AuthService> logger)
{
    public async Task<TokenResultDto> LoginAsync(string email, string password)
    {
        try
        {
            logger.LogInformation("Calling Keycloak token endpoint for {Email}", email);

            var body = await keycloakHelper.GetTokenAsync(email, password)
                ?? throw new UnauthorizedAccessException("Invalid email or password");

            logger.LogInformation("Token received successfully for {Email}", email);

            return new TokenResultDto(
                AccessToken:  body["access_token"].ToString()!,
                RefreshToken: body["refresh_token"].ToString()!
            );
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during login for {Email}", email);
            throw;
        }
    }

    public async Task SignupAsync(SignupRequestDto dto)
    {
        try
        {
            logger.LogInformation("Getting admin token for signup of {Email}", dto.Email);

            var adminToken = await keycloakHelper.GetAdminTokenAsync()
                ?? throw new ArgumentException("Failed to get admin token from Keycloak. Check ClientId and ClientSecret.");

            logger.LogInformation("Admin token received. Creating user {Email} in Keycloak", dto.Email);

            var user = new
            {
                firstName   = dto.FirstName,
                lastName    = dto.LastName,
                email       = dto.Email,
                username    = dto.Email,
                enabled     = true,
                credentials = new[] { new { type = "password", value = dto.Password, temporary = false } }
            };

            var status = await keycloakHelper.CreateUserAsync(adminToken, user);

            if (status == System.Net.HttpStatusCode.Conflict)
                throw new ArgumentException("An account with this email already exists");

            if (!((int)status >= 200 && (int)status < 300))
                throw new ArgumentException($"Signup failed. Keycloak returned: {status}");

            logger.LogInformation("User {Email} created successfully in Keycloak", dto.Email);
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during signup for {Email}", dto.Email);
            throw;
        }
    }
}
