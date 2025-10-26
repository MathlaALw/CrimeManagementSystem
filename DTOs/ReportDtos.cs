namespace Crime_Management_System.DTOs
{
    public class ReportDtos
    {
       public string? Title { get; set; }
       public string? Description { get; set; }
        public string? AreaCit { get; set; }
        decimal? Latitude { get; set; }
        decimal? Longitude { get; set; }
        int? ReportedByUserId { get; set; }
    }
}
