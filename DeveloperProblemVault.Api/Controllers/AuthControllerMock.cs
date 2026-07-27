using DeveloperProblemVault.Api.DTOs.Auth;
using DeveloperProblemVault.Api.Models;
using DeveloperProblemVault.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperProblemVault.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthControllerMock(AuthService authService, ILogger<AuthControllerMock> logger) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
        {
            logger.LogWarning("Login failed - email or password is empty");
            return BadRequest(ResponseHelper.Fail(MessageConstants.LoginFieldsRequired));
        }

        var result = await authService.LoginAsync(dto.Email, dto.Password);
        logger.LogInformation("Login successful for {Email}", dto.Email);
        return Ok(ResponseHelper.Success(MessageConstants.LoginSuccess, result));
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] SignupRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName) ||
            string.IsNullOrWhiteSpace(dto.Email)     || string.IsNullOrWhiteSpace(dto.Password))
        {
            logger.LogWarning("Signup failed - one or more fields are empty");
            return BadRequest(ResponseHelper.Fail(MessageConstants.SignupFieldsRequired));
        }

        await authService.SignupAsync(dto);
        logger.LogInformation("Signup successful for {Email}", dto.Email);
        return StatusCode(201, ResponseHelper.Success(MessageConstants.SignupSuccess));
    }
}
