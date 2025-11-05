using Crime_Management_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Crime_Management_System.DTOs
{
    public class UpdateUserDto
    {
        //  public  string? Username { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Password { get; set; }
        public UserRole? Role { get; set; }
        public ClearanceLevel? ClearanceLevel { get; set; }
    }
}
