using Crime_Management_System.Models;

namespace Crime_Management_System.DTOs
{
    public class CaseDto
    {
        public int Id { get; set; }
        public string CaseNumber { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? AreaCity { get; set; }
        public string? CaseType { get; set; }
        public CaseStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = null!;
        public List<string>? CaseParticipants { get; set; }
        public List<string>? CaseAssignees { get; set; }
        public List<string>? Evidences { get; set; }
        public List<string>? CaseReports { get; set; }
    }
}
