using Crime_Management_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Crime_Management_System.DTOs
{
    public class CreateUserDto
    {
        [Required, MaxLength(60)]
        public string Username { get; set; } = string.Empty;

        [Required, MaxLength(120)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(160)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }

        [Required]
        public ClearanceLevel ClearanceLevel { get; set; }
    }
}
