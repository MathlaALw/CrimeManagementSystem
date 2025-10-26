using Crime_Management_System.Models;
using System.ComponentModel.DataAnnotations;
namespace Crime_Management_System.DTOs
{
    public class CreateTextEvidenceDto
    {
        [Required]
        public int CaseId { get; set; }

        [Required] 
         public string TextContent { get; set; } = string.Empty;
         public string? Remarks { get; set; }
    }
}
