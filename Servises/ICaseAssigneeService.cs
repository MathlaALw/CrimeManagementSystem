
using Crime_Management_System.DTOs;
using Crime_Management_System.Models;

namespace Crime_Management_System.Servises
{
    public interface ICaseAssigneeService
    {
        Task<(bool success, string message)> AssignOfficerAsync(AssignOfficerDto dto);

        Task<List<CaseAssignee>> GetByCaseIdAsync(int caseId);
    }
}