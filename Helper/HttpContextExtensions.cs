using System.Security.Claims;

namespace Crime_Management_System.Helper
{
    public static class HttpContextExtensions
    {
        public static bool TryGetUserId(this ClaimsPrincipal user, out int userId)
        {
            userId = 0;
            var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(id, out userId);
        }
    }
    }
