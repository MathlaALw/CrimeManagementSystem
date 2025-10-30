using Crime_Management_System.Models;

namespace Crime_Management_System.Servises
{
    public interface ITokenService
    {
        (string token, DateTime expiresAtUtc) CreateAccessToken(User user);
    }
}
