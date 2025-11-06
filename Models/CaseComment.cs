using System.ComponentModel.DataAnnotations;

namespace Crime_Management_System.Models
{
    public class CaseComment
    {
        public int Id { get; set; }
        public int CaseId { get; set; }
        public int UserId { get; set; }

        [Required, StringLength(150, MinimumLength = 5)]
        public string CommentText { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User User { get; set; }
        public Case Case { get; set; }
    }
}
