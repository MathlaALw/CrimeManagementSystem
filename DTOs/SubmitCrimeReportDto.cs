using Crime_Management_System.Models;
using System.ComponentModel.DataAnnotations;
namespace Crime_Management_System.DTOs

{
    public class SubmitCrimeReportDto
    {
       public string? Title { get; set; }
       public string? Description { get; set; }
        public string? AreaCit { get; set; }
        decimal? Latitude { get; set; }
        decimal? Longitude { get; set; }
        int? ReportedByUserId { get; set; }
    }
   
}
