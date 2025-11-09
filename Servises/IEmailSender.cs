namespace Crime_Management_System.Servises
{
    public interface IEmailSender
    {
        Task SendAsync(string toEmail, string subject, string htmlBody, string? textBody = null);
        Task SendBulkAsync(IEnumerable<string> toEmails, string subject, string htmlBody, string? textBody = null);

    }
}

