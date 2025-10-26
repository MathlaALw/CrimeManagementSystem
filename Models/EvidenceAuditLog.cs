using System.ComponentModel.DataAnnotations;

namespace Crime_Management_System.Models
{
    public class EvidenceAuditLog
    {
        [Key] 
        public int Id { get; set; }
      

        [Required, MaxLength(20)] 
        public string Action { get; set; } = null!; // add|update|soft_delete|hard_delete
        [Required] 
        public int ActedByUserId { get; set; }
        public User ActedByUser { get; set; } = null!;
        public DateTime ActedAt { get; set; } = DateTime.UtcNow;
        public string? Details { get; set; }

        // navigation properties
        // many-to-one with Evidence
        [Required]
        public int EvidenceId { get; set; }
        public Evidence Evidence { get; set; } = null!;
    }
}
