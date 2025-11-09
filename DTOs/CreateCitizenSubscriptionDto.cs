namespace Crime_Management_System.DTOs
{
    public class CreateCitizenSubscriptionDto
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string City { get; set; } = null!;
        public bool ReceiveNewCrimes { get; set; } = true;
        public bool ReceiveCaseUpdates { get; set; } = true;
        public bool ReceiveAlerts { get; set; } = true;
    }
}
