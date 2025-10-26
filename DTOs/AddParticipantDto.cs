using System.ComponentModel.DataAnnotations;
using Crime_Management_System.Models;
namespace Crime_Management_System.DTOs
{
    public class AddParticipantDto
    {
        [Required]
       public  string FullName { get; set; } = string.Empty;
        public  string? Phone { get; set; }
        public string? Notes { get; set; }
    }
}
