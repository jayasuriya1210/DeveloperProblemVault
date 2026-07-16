namespace DeveloperProblemVault.Api.Config;

public class KeycloakConfig
{
    public string AuthServerUrl     { get; set; } = string.Empty;
    public string Realm             { get; set; } = string.Empty;
    public string ClientId          { get; set; } = string.Empty;
    public string ClientSecret      { get; set; } = string.Empty;
    public string AdminClientId     { get; set; } = string.Empty;
    public string AdminClientSecret { get; set; } = string.Empty;
}
