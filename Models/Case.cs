using System.ComponentModel.DataAnnotations;
using System.Security.Policy;
using System.Threading.Tasks;

namespace Crime_Management_System.Models
{
    public class Case
    {

        [Key] 
        public int Id { get; set; }
        [Required, MaxLength(40)] 
        public string CaseNumber { get; set; } = null!;  // Unique case identifier
        [Required, MaxLength(120)] 
        public string Name { get; set; } = null!;
        [MaxLength(2000)] 
        public string? Description { get; set; }
        [MaxLength(80)] 
        public string? AreaCity { get; set; }
        public string Salt { get; set; }
        [MaxLength(80)] 
        public string? CaseType { get; set; }
        [Required] 
        public ClearanceLevel AuthorizationLevel { get; set; }
        [Required] 
        public CaseStatus Status { get; set; } = CaseStatus.Pending;
       
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // navigation properties

        // one-to-many with User (CreatedBy)

        public int CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; } 
        // one-to-many with CaseParticipant
        public ICollection<CaseParticipant> CaseParticipants { get; set; } = new List<CaseParticipant>();
        // one-to-many with CaseAssignee
        public ICollection<CaseAssignee> CaseAssignees { get; set; } = new List<CaseAssignee>();
        // one-to-many with Evidence
        public ICollection<Evidence> Evidences { get; set; } = new List<Evidence>();
        // one-to-many with CaseReport
        public ICollection<CaseReport> CaseReports { get; set; } = new List<CaseReport>();
   
    }


}
