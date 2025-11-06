using Crime_Management_System.Data;
using Crime_Management_System.Models;
using Crime_Management_System.DTOs;
using Microsoft.EntityFrameworkCore;
using Crime_Management_System.Helper;

namespace Crime_Management_System.Services
{
    public class CaseCommentService
    {
        private readonly CrimeDbContext _db;

        public CaseCommentService(CrimeDbContext db)
        {
            _db = db;
        }

      
        public async Task<bool> CanUserCommentAsync(int userId)
        {
            var oneMinuteAgo = DateTime.UtcNow.AddMinutes(-1);
            int count = await _db.CaseComments
                .Where(c => c.UserId == userId && c.CreatedAt >= oneMinuteAgo)
                .CountAsync();

            return count < 5;
        }

        public async Task<(bool Success, string Message)> AddCommentAsync(int userId, CreateCommentDto dto)
        {

            var validationMessage = CommentValidator.Validate(dto.CommentText);
            if (validationMessage != null)
                return (false, validationMessage);

            if (!await CanUserCommentAsync(userId))
                return (false, "Rate limit exceeded. You can post a maximum of 5 comments per minute.");

            var comment = new CaseComment
            {
                CaseId = dto.CaseId,
                UserId = userId,
                CommentText = dto.CommentText,
                CreatedAt = DateTime.UtcNow
            };

            _db.CaseComments.Add(comment);
            await _db.SaveChangesAsync();

            return (true, "Comment added successfully.");
        }


        public async Task<IEnumerable<CommentResponseDto>> GetCommentsByCaseIdAsync(int caseId)
        {
            return await _db.CaseComments
                .Include(c => c.User)
                .Where(c => c.CaseId == caseId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentResponseDto
                {
                    Id = c.Id,
                    CommentText = c.CommentText,
                    CommentedBy = c.User.FullName,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> DeleteCommentAsync(int commentId, int userId)
        {
            var comment = await _db.CaseComments.FindAsync(commentId);
            if (comment == null)
                return (false, "Comment not found.");
            if (comment.UserId != userId)
                return (false, "You can only delete your own comments.");

            _db.CaseComments.Remove(comment);
            await _db.SaveChangesAsync();

            return (true, "Comment deleted successfully.");
        }
    }
}
