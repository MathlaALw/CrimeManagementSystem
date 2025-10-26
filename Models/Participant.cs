using System.ComponentModel.DataAnnotations;

namespace Crime_Management_System.Models
{
    public class Participant
    {
        [Key] 
        public int Id { get; set; }

        [Required, MaxLength(160)] 
        public string FullName { get; set; } = null!;
        [MaxLength(40)] 
        public string? Phone { get; set; }
        [MaxLength(500)] 
        public string? Notes { get; set; }

        // navigation properties
        // one-to-many with CaseParticipant
        public ICollection<CaseParticipant> CaseParticipants { get; set; } = new List<CaseParticipant>();
    }
}
