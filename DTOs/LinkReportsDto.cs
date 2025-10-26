using Crime_Management_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Crime_Management_System.DTOs

{
    public class LinkReportsDto
    {
        [Required]
       public List<int> ReportIds { get; set; } = new List<int>();
    }
}
