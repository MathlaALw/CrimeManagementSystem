using Crime_Management_System.Models;

namespace Crime_Management_System.DTOs
{
    public class RoleAssignmentDto
    {
        public UserRole Role { get; set; }
        public int ClearanceLevel { get; set; }
    }
}
