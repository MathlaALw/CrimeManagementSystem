using System.ComponentModel.DataAnnotations;

namespace Crime_Management_System.Models
{
    public class CrimeReport
    {
        [Key] public int Id { get; set; }
        [MaxLength(200)] public string? Title { get; set; }
        public string? Description { get; set; }
        [MaxLength(80)] public string? AreaCity { get; set; }
        public DateTime ReportDateTime { get; set; } = DateTime.UtcNow;
        [MaxLength(30)] public string Status { get; set; } = "pending"; // pending|en_route|on_scene|under_investigation|resolved
        public int? ReportedByUserId { get; set; } // null => citizen
        public User? ReportedByUser { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        // navigation properties
        // one-to-many with CaseReport
        public ICollection<CaseReport> CaseReports { get; set; } = new List<CaseReport>();
    }
}
