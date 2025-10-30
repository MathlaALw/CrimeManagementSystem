using Crime_Management_System.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Crime_Management_System.Servises
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwt;
        public TokenService(IOptions<JwtSettings> jwt) => _jwt = jwt.Value;

        public (string token, DateTime expiresAtUtc) CreateAccessToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
                },
                expires: DateTime.UtcNow.AddMinutes(_jwt.AccessTokenMinutes),
                signingCredentials: creds);

            return (new JwtSecurityTokenHandler().WriteToken(token), token.ValidTo);
        }
    }
}
