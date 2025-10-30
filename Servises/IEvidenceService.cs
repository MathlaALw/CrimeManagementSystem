using Crime_Management_System.DTOs;
using Crime_Management_System.Models;

namespace Crime_Management_System.Servises
{
    public interface IEvidenceService
    {
        Task<(int id, string message)?> CreateImageAsync(CreateImageEvidenceDto dto, int actorUserId, string rootPath);
        Task<(int id, string message)?> CreateTextAsync(CreateTextEvidenceDto dto, int actorUserId);
        Task<Evidence?> GetAsync(int id);
        Task<(byte[] bytes, string mime)?> GetImageAsync(int id, string rootPath);
        Task<bool> SoftDeleteAsync(int id, int actorUserId);
        Task<bool> UpdateAsync(int id, UpdateEvidenceDto dto, int actorUserId);
        Task<bool> HardDeleteAsync(int id, int actorUserId, string rootPath);
    }
}