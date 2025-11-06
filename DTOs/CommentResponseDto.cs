namespace Crime_Management_System.DTOs
{
    public class CommentResponseDto
    {
        public int Id { get; set; }
        public string CommentText { get; set; }
        public string CommentedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
