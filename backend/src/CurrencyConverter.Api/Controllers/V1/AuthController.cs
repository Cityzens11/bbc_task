using Asp.Versioning;
using CurrencyConverter.Api.Contracts.Requests;
using CurrencyConverter.Api.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CurrencyConverter.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public sealed class AuthController(IOptions<JwtOptions> jwtOptions) : ControllerBase
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    [AllowAnonymous]
    [HttpPost("token")]
    public IActionResult CreateToken([FromBody] TokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ClientId))
        {
            return BadRequest("ClientId is required.");
        }

        var claims = new List<Claim>
        {
            new("client_id", request.ClientId)
        };

        foreach (var role in request.Roles.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes),
            signingCredentials: credentials);

        var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new { accessToken = encodedToken });
    }
}
