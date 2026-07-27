using DeveloperProblemVault.Api.Helpers;
using DeveloperProblemVault.Api.DTOs.Auth;
using DeveloperProblemVault.Api.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace DeveloperProblemVault.Tests;

public class AuthServiceTests
{
    private readonly Mock<IKeycloakHelper> _keycloakMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _keycloakMock = new Mock<IKeycloakHelper>();
        _loggerMock   = new Mock<ILogger<AuthService>>();
        _service      = new AuthService(_keycloakMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task LoginAsync_ReturnsTokens_WhenCredentialsValid()
    {
        _keycloakMock.Setup(k => k.GetTokenAsync("test@example.com", "pass"))
            .ReturnsAsync(new Dictionary<string, object>
            {
                ["access_token"]  = "access123",
                ["refresh_token"] = "refresh123"
            });

        var result = await _service.LoginAsync("test@example.com", "pass");

        Assert.Equal("access123", result.AccessToken);
        Assert.Equal("refresh123", result.RefreshToken);
    }

    [Fact]
    public async Task LoginAsync_Throws_WhenKeycloakReturnsNull()
    {
        _keycloakMock.Setup(k => k.GetTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((Dictionary<string, object>?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _service.LoginAsync("test@example.com", "wrongpass"));
    }

    [Fact]
    public async Task SignupAsync_Succeeds_WhenKeycloakReturns201()
    {
        _keycloakMock.Setup(k => k.GetAdminTokenAsync()).ReturnsAsync("admintoken");
        _keycloakMock.Setup(k => k.CreateUserAsync("admintoken", It.IsAny<object>()))
            .ReturnsAsync(HttpStatusCode.Created);

        await _service.SignupAsync(new SignupRequestDto("John", "Doe", "john@example.com", "pass"));

        _keycloakMock.Verify(k => k.CreateUserAsync("admintoken", It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task SignupAsync_Throws_WhenAdminTokenIsNull()
    {
        _keycloakMock.Setup(k => k.GetAdminTokenAsync()).ReturnsAsync((string?)null);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.SignupAsync(new SignupRequestDto("John", "Doe", "john@example.com", "pass")));
    }

    [Fact]
    public async Task SignupAsync_Throws_WhenEmailAlreadyExists()
    {
        _keycloakMock.Setup(k => k.GetAdminTokenAsync()).ReturnsAsync("admintoken");
        _keycloakMock.Setup(k => k.CreateUserAsync("admintoken", It.IsAny<object>()))
            .ReturnsAsync(HttpStatusCode.Conflict);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.SignupAsync(new SignupRequestDto("John", "Doe", "john@example.com", "pass")));

        Assert.Contains("already exists", ex.Message);
    }

    [Fact]
    public async Task SignupAsync_Throws_WhenKeycloakReturnsError()
    {
        _keycloakMock.Setup(k => k.GetAdminTokenAsync()).ReturnsAsync("admintoken");
        _keycloakMock.Setup(k => k.CreateUserAsync("admintoken", It.IsAny<object>()))
            .ReturnsAsync(HttpStatusCode.InternalServerError);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.SignupAsync(new SignupRequestDto("John", "Doe", "john@example.com", "pass")));
    }
}
