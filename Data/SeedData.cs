using System.Security.Cryptography;
using System.Text;
using Crime_Management_System.Models;
using Microsoft.EntityFrameworkCore;
namespace Crime_Management_System.Data
{
    public static class SeedData
    {
        public static void seed(CrimeDbContext db)
        {
            if (!db.Users.Any())
            {
                // Admin 
                var salt = Guid.NewGuid().ToString();
                var password = "Admin@123";
                var passwordHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password + salt))
                );

                db.Users.Add(new User
                {
                    Username = "admin",
                    Email = "admin@gmail.com",
                    FullName = "Default Admin",
                    Role = UserRole.Admin,
                    ClearanceLevel = ClearanceLevel.Critical,
                    PasswordHash = passwordHash,
                    Salt = salt,
                    CreatedAt = DateTime.UtcNow
                });


                db.SaveChanges();
                Console.WriteLine("Seeded default admin user.");
            }
            else
            {
                Console.WriteLine("Users already exist. No seeding performed.");
            }
        }
    }
}
