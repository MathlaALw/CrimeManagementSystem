using Crime_Management_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Crime_Management_System.DTOs
{
    public class CreateCaseDto
    {
        public string CaseNumber { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? AreaCity { get; set; }
        public string? CaseType { get; set; }

        [Required]
        public ClearanceLevel AuthorizationLevel { get; set; }

        //[Required]
        //public CaseStatus Status { get; set; }

        //[Required]
        //public int CreatedByUserId { get; set; }

        public List<int> CrimeReportIds { get; set; }

    }

}
