using Crime_Management_System.Models;

namespace Crime_Management_System.Servises
{
    public class NotificationService : INotificationService
    {
        private readonly ICitizenSubscriptionService _subscriptions;
        private readonly IEmailSender _emailSender;

        public NotificationService(
            ICitizenSubscriptionService subscriptions,
            IEmailSender emailSender)
        {
            _subscriptions = subscriptions;
            _emailSender = emailSender;
        }
        //Send new crime report notification

        public async Task SendNewCrimeReportNotificationAsync(CrimeReport report)
        {
            var subscribers = await _subscriptions
                .GetSubscribersForNewCrimesAsync(report.AreaCity);

            if (!subscribers.Any()) return;
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
                    subscribers.Select(x => x.Email),
                    subject,
                    htmlBody);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"[Email Error] {ex.Message}");



            }
        }

        // Send case update notification
        public async Task SendCaseUpdateNotificationAsync(Case caseEntity)
        {
            var subscribers = await _subscriptions
                .GetSubscribersForCaseUpdatesAsync(caseEntity.AreaCity);

            if (!subscribers.Any())
                return;

            var subject = $"Case {caseEntity.CaseNumber} Updated in {caseEntity.AreaCity}";

            var statusText = caseEntity.Status.ToString(); // غيّريها لو الـ Status عندك enum/string

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
                    subscribers.Select(x => x.Email),
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
            var subscribers = await _subscriptions
                .GetSubscribersForAlertsAsync(city);

            if (!subscribers.Any())
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
                    subscribers.Select(x => x.Email),
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