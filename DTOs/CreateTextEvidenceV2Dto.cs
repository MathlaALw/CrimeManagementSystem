namespace Crime_Management_System.DTOs
{
    public class CreateTextEvidenceV2Dto
    {
        public int CaseId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
