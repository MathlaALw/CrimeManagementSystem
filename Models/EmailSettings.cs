namespace Crime_Management_System.Models
{
    public class EmailSettings
    {
        public string Provider { get; set; } = null!;   // "SendGrid"
        public string ApiKey { get; set; } = null!;     // SendGrid API Key
        public string FromEmail { get; set; } = null!;
        public string FromName { get; set; } = null!; 
    }
}
