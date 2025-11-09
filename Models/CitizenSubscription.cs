using System.ComponentModel.DataAnnotations;

namespace Crime_Management_System.Models
{
    public class CitizenSubscription
    {
        [Key]
        public int Id { get; set; }

        // To identify + communicate
        [Required]
        public string FullName { get; set; } = null!;
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        // To limit by city
        [Required]
        public string City { get; set; } = null!;

        // What they want to receive
        [Required]
        public bool ReceiveNewCrimes { get; set; } = true;
        [Required]
        public bool ReceiveCaseUpdates { get; set; } = true;
        [Required]
        public bool ReceiveAlerts { get; set; } = true;

        public DateTime SubscribedAtUtc { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
