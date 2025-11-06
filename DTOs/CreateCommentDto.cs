namespace Crime_Management_System.DTOs
{
    public class CreateCommentDto
    {
        public int CaseId { get; set; }
        public string CommentText { get; set; } = string.Empty;
    }
}
