using System.ComponentModel.DataAnnotations;

namespace Crime_Management_System.DTOs
{
    public class HardDeleteConfirmationDto
    {
        [Required(ErrorMessage = "Confirmation is required")]
        [RegularExpression("^(yes|no)$", ErrorMessage = "Confirmation must be 'yes' or 'no'")]
        public string Confirmation { get; set; }
    }
}
