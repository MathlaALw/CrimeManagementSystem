using System.ComponentModel.DataAnnotations;

namespace Crime_Management_System.Models
{
    public class User
    {
        [Key] 
        public int Id { get; set; }
        [Required, MaxLength(60)] 
        public string Username { get; set; } = null!;
        [Required, MaxLength(120)] 
        public string Email { get; set; } = null!;
        [Required] 
        public string PasswordHash { get; set; } = null!;
        [Required, MaxLength(160)] 
        public string FullName { get; set; } = null!;

    
        [Required] 
        public UserRole Role { get; set; }
        [Required] 
        public ClearanceLevel ClearanceLevel { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
