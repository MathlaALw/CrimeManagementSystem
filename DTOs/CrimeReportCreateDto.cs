using System.ComponentModel.DataAnnotations;

namespace Crime_Management_System.DTOs
{
    public class CrimeReportCreateDto
    {

        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string AreaCity { get; set; }

       // public int? ReportedByUserId { get; set; } 


    }
}
