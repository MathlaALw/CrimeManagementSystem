using Crime_Management_System.Models;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;

namespace Crime_Management_System.Servises
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;
        private readonly SendGridClient _client;

        public EmailSender(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
            _client = new SendGridClient(_settings.ApiKey);

            //Console.WriteLine("=== EMAIL SETTINGS LOADED ===");
            //Console.WriteLine($"Provider:   {_settings.Provider}");
            //Console.WriteLine($"SmtpServer: {_settings.SmtpServer}");
            //Console.WriteLine($"Port:       {_settings.Port}");
            //Console.WriteLine($"Username:   {_settings.Username}");
            //Console.WriteLine($"FromEmail:  {_settings.FromEmail}");
            //Console.WriteLine($"Password length: {_settings.Password?.Length ?? 0}");
            //Console.WriteLine("==============================");
            Console.WriteLine("=== SENDGRID SETTINGS LOADED ===");
            Console.WriteLine($"FromEmail: {_settings.FromEmail}");
            Console.WriteLine($"FromName:  {_settings.FromName}");
            Console.WriteLine("================================");


        }

        // Send a single email
        public async Task SendAsync(string toEmail, string subject, string htmlBody, string? textBody = null)
        {
            var from = new EmailAddress(_settings.FromEmail, _settings.FromName);
            var to = new EmailAddress(toEmail);

            var msg = MailHelper.CreateSingleEmail(
                from,
                to,
                subject,
                textBody ?? string.Empty, // plain text
                htmlBody                  // html
            );

            var response = await _client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Body.ReadAsStringAsync();
                Console.WriteLine($"[SendGrid Error] Status: {response.StatusCode}, Body: {body}");
            }
        }
        // Send bulk email -> Multiple recipients

        public async Task SendBulkAsync(IEnumerable<string> toEmails, string subject, string htmlBody, string? textBody = null)
        {
            var from = new EmailAddress(_settings.FromEmail, _settings.FromName);
            var tos = toEmails.Distinct().Select(e => new EmailAddress(e)).ToList();

            // Create the SendGrid message
            var msg = new SendGridMessage
            {
                From = from,
                Subject = subject,
                PlainTextContent = textBody ?? string.Empty,
                HtmlContent = htmlBody
            };

            foreach (var to in tos)
            {
                msg.AddTo(to);
            }

            var response = await _client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Body.ReadAsStringAsync();
                Console.WriteLine($"[SendGrid Bulk Error] Status: {response.StatusCode}, Body: {body}");
            }
        }
    }
}