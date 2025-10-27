using System.ComponentModel.DataAnnotations;

namespace Crime_Management_System.Models
{
    public class User
    {
        [Key] 
        public int Id { get; set; }
        [Required, MaxLength(60)] 
        public string Username { get; set; } = null!;
        [Required, MaxLength(120)] 
        public string Email { get; set; } = null!;
        [Required] 
        public string PasswordHash { get; set; } = null!;
        [Required, MaxLength(160)] 
        public string FullName { get; set; } = null!;

    
        [Required] 
        public UserRole Role { get; set; }
        [Required] 
        public ClearanceLevel ClearanceLevel { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // navigation properties
        // one-to-many with CaseAssignees
        public ICollection<CaseAssignee> CaseAssignees { get; set; } = new List<CaseAssignee>();

        // one-to-many with CrimeReports
        public ICollection<CrimeReport> CrimeReports { get; set; } = new List<CrimeReport>();

        // one-to-many with Cases
        public ICollection<Case> CreatedCases { get; set; } = new List<Case>();

        // one-to-many with Evidences
        public ICollection<Evidence> AddedEvidences { get; set; } = new List<Evidence>();

        // one-to-many with EvidenceAuditLogs
        public ICollection<EvidenceAuditLog> EvidenceAuditLogs { get; set; } = new List<EvidenceAuditLog>();
        public bool IsActive { get; internal set; }
    }
}
