using Crime_Management_System.Models;
using Microsoft.EntityFrameworkCore;
namespace Crime_Management_System.Data
{
    public static class SeedData
    {
        public static void seed(CrimeDbContext db)
        {
            db.Database.Migrate();
            // Seed default admin user if no users exist
            if (!db.Users.Any())
            {
                db.Users.Add(new User
                {
                    Username = "admin",
                    Email = "admin@crime.local",
                    FullName = "Default Admin",
                    Role = UserRole.Admin,
                    ClearanceLevel = ClearanceLevel.Critical,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
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
