namespace DeveloperProblemVault.Api.DTOs.Auth;

public record LoginRequestDto(string Email, string Password);

public record SignupRequestDto(string FirstName, string LastName, string Email, string Password);

public record TokenResultDto(string AccessToken, string RefreshToken);
