using Crime_Management_System.Data;
using Crime_Management_System.DTOs;
using Crime_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Crime_Management_System.Servises
{
    public class NotificationService : INotificationService
    {
        //private readonly ICitizenSubscriptionService _subscriptions;
        private readonly IEmailSender _emailSender;
        private readonly CrimeDbContext _db;
        private readonly ICitizenDirectoryClient _citizenDirectoryClient;
        public NotificationService(
            ICitizenDirectoryClient citizenDirectoryClient,
            IEmailSender emailSender , CrimeDbContext crimeDbContext)
        {
            //_subscriptions = subscriptions;
            _emailSender = emailSender;
            _db = crimeDbContext;
            _citizenDirectoryClient = citizenDirectoryClient;
        }
        //Send new crime report notification 
        //New Crime Report -> notify Admins + Investigators + subscribed citizens 

        public async Task SendNewCrimeReportNotificationAsync(CrimeReport report)
        {
            // Internal users : Admins + Investigators
            var internalRecipients = await _db.Users
                .Where(u => u.IsActive &&
                            (u.Role == UserRole.Admin ||
                             u.Role == UserRole.Investigator))
                .Select(u => u.Email)
                .ToListAsync();

            // citizens subscribed to "new crimes" in this city
            //var city = report.AreaCity ?? string.Empty;
            //var subscribers = await _subscriptions
            //    .GetSubscribersForNewCrimesAsync(city);
          

            //// Extract emails
            //var subscriberEmails = subscribers
            //    .Select(x => x.Email)
            //    .ToList();

            //// Combine and deduplicate
            //var allRecipients = internalRecipients
            //    .Concat(subscriberEmails)
            //    .Distinct()
            //    .ToList();

            // No recipients, no email
            //if (!allRecipients.Any())
            //{
            //    Console.WriteLine($"[Email Info] No recipients found for city '{city}'. Skipping email.");
            //    return;
            //}

            var subject = $"New Crime Reported in {report.AreaCity}";


            var htmlBody = $@"
                        <h2 style='color:#c0392b;'>New Crime Alert</h2>
                        <p><strong>Title:</strong> {report.Title}</p>
                        <p><strong>Description:</strong> {report.Description}</p>
                        <p><strong>Reported At:</strong> {report.ReportDateTime}</p>
                        <p><strong>Location:</strong> {report.AreaCity}</p>
                        <hr/>
                        <p style='color:#555;'>This is an automated alert from District Core Crime Management System.</p>
                    ";
            try
            {
                await _emailSender.SendBulkAsync(
                    internalRecipients, 
                    subject,
                    htmlBody);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"[Email Error] {ex.Message}");



            }
        }

        // Send case update notification
        // Case Update -> notify Citizens + Assigned Officers
        public async Task SendCaseUpdateNotificationAsync(Case caseEntity)
        {
            // Get city from case
            var city = caseEntity.AreaCity ?? string.Empty;


            //// Citizens subscribed for case updates in this city
            //var subscribers = await _subscriptions
            //    .GetSubscribersForCaseUpdatesAsync(city);

            //var subscriberEmails = subscribers
            //    .Select(x => x.Email)
            //    .Where(e => !string.IsNullOrWhiteSpace(e))
            //    .ToList();

            // Officers assigned to this case
            var officerEmails = await _db.CaseAssignees
                .Include(a => a.User)
                .Where(a => a.CaseId == caseEntity.Id &&
                            a.User.IsActive &&
                            a.User.Role == UserRole.Officer)
                .Select(a => a.User.Email)
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .ToListAsync();
            //// get All Emails
            //var allRecipients = subscriberEmails
            //   .Concat(officerEmails)
            //   .Distinct()
            //   .ToList();

            if (!officerEmails.Any())
                return;

            var subject = $"Case {caseEntity.CaseNumber} Updated in {caseEntity.AreaCity}";

            var statusText = caseEntity.Status.ToString(); 

            var htmlBody = $@"
                <h2>Case Status Update</h2>
                <p><strong>Case Number:</strong> {caseEntity.CaseNumber}</p>
                <p><strong>Case Name:</strong> {caseEntity.Name}</p>
                <p><strong>New Status:</strong> {statusText}</p>
                <p><strong>Last Updated:</strong> {caseEntity.CreatedAt}</p>
                <hr/>
                <p style='color:#555;'>You are receiving this email because you subscribed to case updates in {caseEntity.AreaCity}.</p>
            ";
            try
            {
                await _emailSender.SendBulkAsync(
                    officerEmails,
                    subject,
                    htmlBody);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"[Email Error] {ex.Message}");
               
            }
        }

        // Send community alert
        public async Task SendCommunityAlertAsync(string city, string title, string message)
        {
            //var subscribers = await _subscriptions
            //    .GetSubscribersForAlertsAsync(city);

            var emails = await _citizenDirectoryClient.GetCitizenEmailsAsync(
                new CitizenEmailFilterRequestDto
                {
                    City = city,
                   
                });

            if (!emails.Any())
                return;

            var subject = $"📢 Community Alert for {city}: {title}";

            var htmlBody = $@"
                <h2>Community Alert - {city}</h2>
                <h3>{title}</h3>
                <p>{message}</p>
                <hr/>
                <p style='color:#555;'>Stay safe. This alert was sent by District Core Crime Management System.</p>
            ";
            try
            {
                await _emailSender.SendBulkAsync(
                    emails,
                    subject,
                    htmlBody);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Email Error] {ex.Message}");
            }
        }
    }
}