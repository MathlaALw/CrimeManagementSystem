using Crime_Management_System.Models;
using System.ComponentModel.DataAnnotations;
namespace Crime_Management_System.DTOs
{
    public class CreateImageEvidenceDto
    {
        [Required]
         public int CaseId { get; set; }
        [Required] 
        public IFormFile Image { get; set; } = null!;
        public  string? Remarks { get; set; }
    }
}
