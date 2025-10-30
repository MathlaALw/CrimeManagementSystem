// Controllers/DiagController.cs (TEMPORARY - remove later)
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Crime_Management_System.Models;
using System.Security.Claims;

[ApiController]
[Route("api/diag")]
public class DiagController : ControllerBase
{
    private readonly JwtSettings _jwt;
    public DiagController(IOptions<JwtSettings> jwt) => _jwt = jwt.Value;

    [HttpGet("jwt-settings")]
    public IActionResult Show()
    {
        using var sha = SHA256.Create();
        var keyHash = Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(_jwt.Key)));
        return Ok(new
        {
            Issuer = _jwt.Issuer,
            Audience = _jwt.Audience,
            KeySha256 = keyHash
        });
    }

    [HttpPost("validate")]
    public IActionResult Validate([FromBody] string token)
    {
        var tvp = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwt.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.NameIdentifier
        };

        var handler = new JwtSecurityTokenHandler();
        try
        {
            var principal = handler.ValidateToken(token, tvp, out var _);
            return Ok(new { valid = true, name = principal.Identity?.Name, roles = principal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value) });
        }
        catch (Exception ex)
        {
            return BadRequest(new { valid = false, error = ex.GetType().Name, message = ex.Message });
        }

    }
        [HttpPost("sign")]
        public IActionResult Sign([FromBody] string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_jwt.Issuer, _jwt.Audience,
                new[] { new Claim(ClaimTypes.Name, username), new Claim(ClaimTypes.Role, "Officer") },
                expires: DateTime.UtcNow.AddMinutes(_jwt.AccessTokenMinutes),
                signingCredentials: creds);
            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }

