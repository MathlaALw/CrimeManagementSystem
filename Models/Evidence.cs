using System.ComponentModel.DataAnnotations;

namespace Crime_Management_System.Models
{
    public class Evidence
    {

        [Key] 
        public int Id { get; set; }
       
        [Required] 
        public EvidenceType Type { get; set; }
        // For Text
        public string? TextContent { get; set; }
        // For Image
        public string? FileUrl { get; set; }
        public string? MimeType { get; set; }
        public long? SizeBytes { get; set; }


        public string? Remarks { get; set; }
        public bool IsSoftDeleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // navigation properties
        // one-to-many with EvidenceAuditLog
        public ICollection<EvidenceAuditLog> AuditLogs { get; set; } = new List<EvidenceAuditLog>();
        // many-to-one with Case
        [Required]
        public int CaseId { get; set; }
        public Case Case { get; set; } = null!;

        // many-to-one with User (AddedBy)
        [Required]
        public int AddedByUserId { get; set; }
        public User AddedByUser { get; set; } = null!;


    }
}
