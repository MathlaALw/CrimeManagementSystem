using Crime_Management_System.DTOs;

namespace Crime_Management_System.Servises
{
    public class CitizenDirectoryClient : ICitizenDirectoryClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CitizenDirectoryClient> _logger;


        public CitizenDirectoryClient(HttpClient httpClient, ILogger<CitizenDirectoryClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        // Get citizen emails from CitizenManagementSystem
        public async Task<List<string>> GetCitizenEmailsAsync(CitizenEmailFilterRequestDto filter)
        {
            // POST https://citizen-service/api/Citizen/emails
            var response = await _httpClient.PostAsJsonAsync("api/Citizen/emails", filter);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("CitizenService returned {StatusCode}", response.StatusCode);
                return new List<string>();
            }

            var emails = await response.Content.ReadFromJsonAsync<List<string>>();
            return emails ?? new List<string>();
        }
    }
}
