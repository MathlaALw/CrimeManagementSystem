namespace Crime_Management_System.DTOs
{
    public class CreateAlertDto
    {
        public string City { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}
