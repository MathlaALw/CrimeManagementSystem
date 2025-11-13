namespace Crime_Management_System.DTOs
{
    public class CommunityAlertFromCitizenServiceDto
    {
        public string? City { get; set; }   
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
