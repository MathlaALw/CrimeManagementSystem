using System;
using Crime_Management_System;
using Crime_Management_System.DTOs;
namespace Crime_Management_System.DTOs
    
{
    public class UserDtos
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public ClearanceLevel ClearanceLevel { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
