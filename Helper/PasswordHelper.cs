using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System;

namespace Crime_Management_System.Helper
{
    public class PasswordHelper
    {

        public static byte[] GenerateSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        // Hash password with salt using PBKDF2
        public static string HashPassword(string password, byte[] salt)
        {
            // Derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return hashed;
        }

        // Verify password against stored hash and salt
        public static bool VerifyPassword(string password, string storedHash, byte[] storedSalt)
        {
            string hashedPassword = HashPassword(password, storedSalt);
            return hashedPassword == storedHash;

        }
    }
}
