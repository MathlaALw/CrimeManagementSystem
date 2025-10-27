using Crime_Management_System.Models;
using System.ComponentModel.DataAnnotations;
namespace Crime_Management_System.DTOs

{
    public class SubmitCrimeReportDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
       public string? Description { get; set; }
        public string? AreaCity { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int? ReportedByUserId { get; set; }
    }
   
}
