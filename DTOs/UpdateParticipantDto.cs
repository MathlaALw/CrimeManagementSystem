using Crime_Management_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Crime_Management_System.DTOs
{
    public class UpdateParticipantDto
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Phone { get; set; }

        public string? Notes { get; set; }


    }
}
