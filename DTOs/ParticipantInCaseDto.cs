using Crime_Management_System.Models;

namespace Crime_Management_System.DTOs
{
    public class ParticipantInCaseDto
    {
        public int ParticipantId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Notes { get; set; }

        public ParticipantRole Role { get; set; }

        public DateTime AddedAt { get; set; }
        public int? AddedByUserId { get; set; }
    }
}
