using Crime_Management_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Crime_Management_System.DTOs
{
    public class CreateCaseDto
    {
        [Required(ErrorMessage = "Case number is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Case number must be between 3 and 50 characters.")]
        public string CaseNumber { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Description must be at least 10 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Area or city is required.")]
        public string AreaCity { get; set; }

        [Required(ErrorMessage = "Case type is required.")]
        public string CaseType { get; set; }

        [Required(ErrorMessage = "Authorization level is required.")]
        [RegularExpression("^(Low|Medium|High)$", ErrorMessage = "Authorization level must be 'Low', 'Medium', or 'High'.")]
        public string AuthorizationLevel { get; set; }

        [Required(ErrorMessage = "At least one crime report ID is required.")]
        [MinLength(1, ErrorMessage = "At least one crime report ID must be included.")]
        public List<int> CrimeReportIds { get; set; }

    }

}
