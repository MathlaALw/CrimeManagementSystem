using Crime_Management_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Crime_Management_System.DTOs
{
    public class CreateCaseDto
    {
        [Required, MaxLength(40)]
        public string CaseNumber { get; set; } = null!;

        [Required, MaxLength(120)]
        public string Name { get; set; } = null!;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(80)]
        public string? AreaCity { get; set; }

        [MaxLength(80)]
        public string? CaseType { get; set; }

        [Required]
        public ClearanceLevel AuthorizationLevel { get; set; }

        [Required]
        public CaseStatus Status { get; set; }

        [Required]
        public int CreatedByUserId { get; set; }
        public List<int> CrimeReportIds { get; set; }

    }

}
