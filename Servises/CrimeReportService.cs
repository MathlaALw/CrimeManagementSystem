using Crime_Management_System.DTOs;
using Crime_Management_System.Models;
using Crime_Management_System.Repos;

namespace Crime_Management_System.Servises
{
    public class CrimeReportService : ICrimeReportService
    {
        private readonly ICrimeReportRepository _crimeReportRepository;

        public CrimeReportService(ICrimeReportRepository crimeReportRepository)
        {
            _crimeReportRepository = crimeReportRepository;
        }

        // CRUD operations for CrimeReport
        // Get report by ID
        public async Task<CrimeReport> GetReportByIdAsync(int id)
        {
            return await _crimeReportRepository.GetByIdAsync(id);
        }
        // Get report byReport ID
        public async Task<CrimeReport> GetReportByPublicIdAsync(int reportId)
        {
            return await _crimeReportRepository.GetByReportIdAsync(reportId);
        }

        // Create a new report
        public async Task<CrimeReport> CreateReportAsync(SubmitCrimeReportDto reportDto)
        {

            var report = new CrimeReport
            {
                Id = reportDto.Id,
                Title = reportDto.Title,
                Description = reportDto.Description,
                AreaCity = reportDto.AreaCity,
                Latitude = reportDto.Latitude,
                Longitude = reportDto.Longitude,
                ReportedByUserId = reportDto.ReportedByUserId,

            };

            return await _crimeReportRepository.CreateAsync(report);
        }

        // Update an existing report
        public async Task<CrimeReport> UpdateReportAsync(int id, CrimeReportUpdateDto reportDto)
        {
            var report = await _crimeReportRepository.GetByIdAsync(id);
            if (report == null) throw new ArgumentException("Report not found");

            report.Title = reportDto.Title;
            report.Description = reportDto.Description;
            report.AreaCity = reportDto.AreaCity;
            report.Status = reportDto.Status;



            return await _crimeReportRepository.UpdateAsync(report);
        }

        // check report status and link to case
        public async Task<bool> LinkReportToCaseAsync(int reportId, int caseId)
        {
            var report = await _crimeReportRepository.GetByIdAsync(reportId);
            if (report == null) return false;

            report.Id = caseId;
            report.Status = "UnderInvestigation";
            await _crimeReportRepository.UpdateAsync(report);
            return true;
        }

        // Get all pending reports
        public async Task<IEnumerable<CrimeReport>> GetPendingReportsAsync()
        {
            return await _crimeReportRepository.GetPendingReportsAsync();
        }

        // Get report status by report ID
        public async Task<string> GetReportStatusAsync(int reportId)
        {
            var report = await _crimeReportRepository.GetByReportIdAsync(reportId);
            return report?.Status ?? "Not Found";
        }
    }
}
