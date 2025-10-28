namespace Crime_Management_System.DTOs
{
    public class EvidenceDto
    {
        public int Id { get; set; }
        public int CaseId { get; set; }
        public string? FilePath { get; set; }      // for images/files
        public string? TextContent { get; set; }   // for text evidence
        public string? Remarks { get; set; }
        public string? Type { get; set; }          // e.g. "Image" or "Text"
        public DateTime CreatedAtUtc { get; set; }
        public int CreatedByUserId { get; set; }
    }
}
