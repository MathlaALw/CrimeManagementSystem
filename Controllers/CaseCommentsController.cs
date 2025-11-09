using Crime_Management_System.Data;
using Crime_Management_System.DTOs;
using Crime_Management_System.Models;
using Crime_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Crime_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Officer, Investigator")]
    public class CaseCommentsController : ControllerBase
    {
        private readonly CaseCommentService _commentService;
        private readonly CrimeDbContext _db;

        public CaseCommentsController(CaseCommentService commentService, CrimeDbContext db)
        {
            _commentService = commentService;
            _db = db;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddComment(CreateCommentDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid comment data.");

            var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User ID not found in token.");

            int userId = int.Parse(userIdClaim);

            if (_db == null)
                return StatusCode(500, "Database context is not initialized.");

            if (_db.CaseComments == null)
                return StatusCode(500, "CaseComments DbSet not configured in DbContext.");

            // Validation
            if (string.IsNullOrWhiteSpace(dto.CommentText))
                return BadRequest("Comment cannot be empty.");

            if (dto.CommentText.Length < 5)
                return BadRequest("Comment must be at least 5 characters long.");

            if (dto.CommentText.Length > 150)
                return BadRequest("Comment cannot exceed 150 characters.");

            if (dto.CommentText.Contains("<") || dto.CommentText.Contains(">"))
                return BadRequest("HTML tags are not allowed in comments.");

            if (!System.Text.RegularExpressions.Regex.IsMatch(dto.CommentText, @"^[a-zA-Z0-9\s.,!?'-]*$"))
                return BadRequest("Comment contains invalid characters. Please use only letters, numbers, and basic punctuation.");
            //check if there case 

            var existingCase = await _db.Cases.FirstOrDefaultAsync(c => c.Id == dto.CaseId);
            if (existingCase == null)
            {
                return NotFound(new
                {
                    Error = $"Case with ID {dto.CaseId} does not exist. Please verify the Case ID before adding a comment."
                });
            }
            // Rate limiting
            var oneMinuteAgo = DateTime.UtcNow.AddMinutes(-1);
            int recentComments = await _db.CaseComments
                .Where(c => c.UserId == userId && c.CreatedAt >= oneMinuteAgo)
                .CountAsync();

            if (recentComments >= 5)
                return BadRequest("Rate limit exceeded. You can only post up to 5 comments per minute.");

            try { 
            var comment = new CaseComment
            {
                CaseId = dto.CaseId,
                UserId = userId,
                CommentText = dto.CommentText,
                CreatedAt = DateTime.UtcNow
            };

            _db.CaseComments.Add(comment);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                Message = "Comment added successfully.",
                CommentId = comment.Id,
                comment.CaseId,
                comment.UserId,
                comment.CreatedAt
            });
        }
            catch (DbUpdateException ex)
            {
           
                return BadRequest(new
                {
                    Error = "Failed to add comment. The specified case may not exist or has been deleted.",
                    Details = ex.InnerException?.Message ?? ex.Message
                });
            }
            catch (Exception ex)
            {
                //
                return StatusCode(500, new
                {
                    Error = "An unexpected error occurred while adding the comment.",
                    Details = ex.Message
                });
            }
        }

        [HttpGet("GetComments")]
        public async Task<IActionResult> GetComments(int caseId)
        {
            try
            {

                if (caseId <= 0)
                    return BadRequest("Invalid Case ID. Case ID must be greater than zero.");
               var existingCase = await _db.Cases.FirstOrDefaultAsync(c => c.Id == caseId);
                if (existingCase == null)
                    return NotFound(new
                    {
                        Error = $"Case with ID {caseId} does not exist."
                    });

                var comments = await _commentService.GetCommentsByCaseIdAsync(caseId);
                if (comments == null || !comments.Any())
                {
                    return Ok(new
                    {
                        Message = "No comments found for this case.",
                        Comments = new List<object>()
                    });
                }

                return Ok(new
                {
                    Message = "Comments retrieved successfully.",
                    TotalComments = comments.Count(),
                    Comments = comments
                });
            }
            catch (Exception ex)
            {                return StatusCode(500, new
                {
                    Error = "An unexpected error occurred while retrieving comments.",
                    Details = ex.Message
                });
            }
        }

        [HttpDelete("DeleteComment")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = await _commentService.DeleteCommentAsync(commentId, userId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }
    }
}
