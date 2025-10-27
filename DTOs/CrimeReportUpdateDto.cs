namespace Crime_Management_System.DTOs
{
    public class CrimeReportUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? AreaCity { get; set; }
        public string? Status { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}
