
namespace Crime_Management_System.Models

{
    public class AuthResponse
    {
        public string Token { get; set; }
        public string Role { get; set; }
        public string ClearanceLevel { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}

