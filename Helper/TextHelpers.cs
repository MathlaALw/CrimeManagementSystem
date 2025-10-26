namespace Crime_Management_System.Helper
{
    public static class TextHelpers
    {
        // Truncates a string at the last whole word before maxChars and appends " ..."
        public static string TruncateAtWord(string? input, int maxChars)
        {

            // Handle null or empty input
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            // If input is shorter than maxChars, return it as is
            if (input.Length <= maxChars) return input;
            var safe = input.Substring(0, maxChars); // Cut to maxChars
            var lastSpace = safe.LastIndexOf(' '); // Find last space
            // If there's a space, cut to that point to avoid breaking a word
            if (lastSpace > 0) safe = safe.Substring(0, lastSpace);
            // Append ellipsis
            return safe + " ...";
        }
    }
}
