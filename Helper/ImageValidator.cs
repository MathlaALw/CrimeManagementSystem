namespace Crime_Management_System.Helper
{
    public static class ImageValidator
    {
        // Allowed MIME types for images
        private static readonly HashSet<string> AllowedMime = new() 
        { 
            "image/png", 
            "image/jpeg",
            "image/gif",
            "image/webp" 
        };

        public static bool IsValidImage(IFormFile file) // IFormFile -> Represents a file uploaded via an HTTP form (used in ASP.NET Core for uploads).
        {
            // Check if file is not null, has content, and its MIME type is in the allowed list
            return file.Length > 0 && AllowedMime.Contains(file.ContentType);
        }

    }
}
