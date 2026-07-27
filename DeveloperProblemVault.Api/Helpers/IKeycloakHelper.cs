using System.Net;

namespace DeveloperProblemVault.Api.Helpers;

public interface IKeycloakHelper
{
    Task<Dictionary<string, object>?> GetTokenAsync(string email, string password);
    Task<string?> GetAdminTokenAsync();
    Task<HttpStatusCode> CreateUserAsync(string adminToken, object user);
}
