using Microsoft.AspNetCore.Identity;

namespace Crime_Management_System.Servises
{
    public class PasswordService : IPasswordService
    {
        private readonly PasswordHasher<string> _hasher = new();
        public string Hash(string password) => _hasher.HashPassword("", password);
        public bool Verify(string hash, string password) =>
            _hasher.VerifyHashedPassword("", hash, password) is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
