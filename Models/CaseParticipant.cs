using System.ComponentModel.DataAnnotations;

namespace Crime_Management_System.Models
{
    public class CaseParticipant
    {
        [Key] 
        public int Id { get; set; }
        [Required] 
        public int CaseId { get; set; }
      
        [Required] 
        public ParticipantRole Role { get; set; }
        public int? AddedByUserId { get; set; }
        public User? AddedByUser { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;


        public Case Case { get; set; } = null!;
        [Required]
        public int ParticipantId { get; set; }
        public Participant Participant { get; set; } = null!;
    }
}
