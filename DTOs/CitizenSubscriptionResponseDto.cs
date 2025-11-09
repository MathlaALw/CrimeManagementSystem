namespace Crime_Management_System.DTOs
{
    public class CitizenSubscriptionResponseDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string City { get; set; } = null!;
    }
}
