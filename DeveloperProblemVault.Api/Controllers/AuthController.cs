using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DeveloperProblemVault.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IConfiguration config) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequestDto dto)
    {
        var validUsername = config["Auth:Username"];
        var validPassword = config["Auth:Password"];

        if (dto.Username != validUsername || dto.Password != validPassword)
            return Unauthorized(new ResponseModel { Stat = 0, Message = "Failed", Reason = "Invalid username or password" });

        var token = GenerateToken(dto.Username);
        return Ok(new ResponseModel { Stat = 1, Message = "Login successful", Result = new { token } });
    }

    private string GenerateToken(string username)
    {
        var key     = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds   = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry  = DateTime.UtcNow.AddMinutes(double.Parse(config["Jwt:ExpiryMinutes"]!));

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer:             config["Jwt:Issuer"],
            audience:           config["Jwt:Audience"],
            claims:             claims,
            expires:            expiry,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record LoginRequestDto(string Username, string Password);
