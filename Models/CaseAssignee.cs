using System.ComponentModel.DataAnnotations;

namespace Crime_Management_System.Models
{
    public class CaseAssignee
    {
        [Key] 
        public int Id { get; set; }

     
        [Required, MaxLength(20)] 
        public string AssignedRole { get; set; } = "Officer"; // Investigator | Officer
        [Required, MaxLength(20)] 
        public string ProgressStatus { get; set; } = "pending"; // pending|ongoing|closed
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        // navigation properties
        // many-to-one with Case
        [Required]
        public int CaseId { get; set; }
        public Case Case { get; set; } = null!;
        // many-to-one with User
        [Required]
        public int UserId { get; set; }
        public User User { get; set; } = null!;


    }
}
