using System.ComponentModel.DataAnnotations;

namespace Crime_Management_System.Models
{
    public class CaseReport
    {
        [Key] public int Id { get; set; }
     
        public DateTime LinkedAt { get; set; } = DateTime.UtcNow;

        // navigation properties
        // many-to-one with Case
        [Required] public int CaseId { get; set; }
        public Case Case { get; set; } = null!;
        // many-to-one with CrimeReport
        [Required] public int ReportId { get; set; }
        public CrimeReport Report { get; set; } = null!;
    }
}
