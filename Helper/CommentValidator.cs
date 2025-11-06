namespace Crime_Management_System.Helper
{
    public class CommentValidator
    {
        public static string? Validate(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length < 5)
                return "Comment must be at least 5 characters long.";
            if (text.Length > 150)
                return "Comment cannot exceed 150 characters.";
            if (System.Text.RegularExpressions.Regex.IsMatch(text, "<.*?>"))
                return "HTML tags are not allowed in comments.";
            if (!System.Text.RegularExpressions.Regex.IsMatch(text, @"^[a-zA-Z0-9\s.,!?'\-]*$"))
                return "Comment contains invalid characters. Please use only letters, numbers, and basic punctuation.";
            return null;
        }
    }
}
