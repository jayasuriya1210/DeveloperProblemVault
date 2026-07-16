using DeveloperProblemVault.Api.DTOs.Auth;
using DeveloperProblemVault.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperProblemVault.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(AuthService authService, ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        logger.LogInformation("Login attempt for {Email}", dto.Email);

        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
        {
            logger.LogWarning("Login failed - email or password is empty");
            return BadRequest(Fail("Email and password are required"));
        }

        try
        {
            var result = await authService.LoginAsync(dto.Email, dto.Password);
            logger.LogInformation("Login successful for {Email}", dto.Email);
            return Ok(new ResponseModel { Stat = 1, Message = "Login successful", Result = result });
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning("Login failed for {Email} - {Reason}", dto.Email, ex.Message);
            return Unauthorized(Fail(ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during login for {Email}", dto.Email);
            return StatusCode(500, Fail("An unexpected error occurred"));
        }
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] SignupRequestDto dto)
    {
        logger.LogInformation("Signup attempt for {Email}", dto.Email);

        if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName) ||
            string.IsNullOrWhiteSpace(dto.Email)     || string.IsNullOrWhiteSpace(dto.Password))
        {
            logger.LogWarning("Signup failed - one or more fields are empty");
            return BadRequest(Fail("All fields are required"));
        }

        try
        {
            await authService.SignupAsync(dto);
            logger.LogInformation("Signup successful for {Email}", dto.Email);
            return StatusCode(201, new ResponseModel { Stat = 1, Message = "User registered successfully" });
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning("Signup failed for {Email} - {Reason}", dto.Email, ex.Message);
            return BadRequest(Fail(ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during signup for {Email}", dto.Email);
            return StatusCode(500, Fail("An unexpected error occurred"));
        }
    }

    private static ResponseModel Fail(string reason) =>
        new() { Stat = 0, Message = MessageConstants.Failed, Reason = reason };
}
