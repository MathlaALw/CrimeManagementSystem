
namespace Crime_Management_System.Models
{
    public class JwtSettings
    {

        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public string Key { get; set; } = null!; 
        public int AccessTokenMinutes { get; set; } = 60;
    }
}
