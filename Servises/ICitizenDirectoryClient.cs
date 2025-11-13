using Crime_Management_System.DTOs;

namespace Crime_Management_System.Servises
{
    public interface ICitizenDirectoryClient
    {
        Task<List<string>> GetCitizenEmailsAsync(CitizenEmailFilterRequestDto filter);
    }
}