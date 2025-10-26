using Crime_Management_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Crime_Management_System.DTOs
{
    public class AddParticipantToCaseDto
    {
        [Required]
        public int ParticipantId { get; set; }

        [Required]
        public ParticipantRole Role { get; set; }
    }
}
