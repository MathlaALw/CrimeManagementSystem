using Crime_Management_System.Models;

namespace Crime_Management_System.Servises
{
    public interface INotificationService
    {
        Task SendCaseUpdateNotificationAsync(Case caseEntity);
        Task SendCommunityAlertAsync(string city, string title, string message);
        Task SendNewCrimeReportNotificationAsync(CrimeReport report);
    }
}