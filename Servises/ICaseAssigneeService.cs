
using Crime_Management_System.DTOs;

namespace Crime_Management_System.Servises
{
    public interface ICaseAssigneeService
    {
        Task<(bool success, string message)> AssignOfficerAsync(AssignOfficerDto dto);
    }
}