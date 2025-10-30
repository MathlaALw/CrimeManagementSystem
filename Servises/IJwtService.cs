using Crime_Management_System.Models;
using System.Security.Claims;

namespace Crime_Management_System.Servises
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        ClaimsPrincipal ValidateToken(string token);
    }
}