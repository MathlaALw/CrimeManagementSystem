using Crime_Management_System.Data;
using Crime_Management_System.DTOs;
using Crime_Management_System.Helper;
using Crime_Management_System.Models;
using Crime_Management_System.Repos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace Crime_Management_System.Servises
{
    public class EvidenceService : IEvidenceService
    {
        private readonly IEvidenceRepository _repo;
        private readonly CrimeDbContext _db;
      

        public EvidenceService(IEvidenceRepository repo, CrimeDbContext db)
        {
            _repo = repo;
            _db = db;
        }

        // Create text evidence
        public async Task<(int id, string message)?> CreateTextAsync(CreateTextEvidenceDto dto, int actorUserId)
        {
            if (!await _db.Cases.AnyAsync(c => c.Id == dto.CaseId)) return null;

            var e = new Evidence
            {
                CaseId = dto.CaseId,
                AddedByUserId = actorUserId,
                Type = EvidenceType.Text,
                TextContent = dto.TextContent,
                Remarks = dto.Remarks
            };

   
            _db.EvidenceAuditLogs.Add(new EvidenceAuditLog
            {
                Evidence = e,
                ActedByUserId = actorUserId,
                Action = "add",
                Details = "text"
            });

            await _repo.SaveAsync();
            return (e.Id, "Text evidence recorded");
        }

        // Create image evidence
        public async Task<(int id, string message)?> CreateImageAsync(CreateImageEvidenceDto dto, int actorUserId, string rootPath)
        {
            if (!await _db.Cases.AnyAsync(c => c.Id == dto.CaseId)) return null;
            if (dto.Image == null || !ImageValidator.IsValidImage(dto.Image)) return null;

            // Save image to disk
            var folder = Path.Combine(rootPath, "uploads"); // Ensure this folder exists
            Directory.CreateDirectory(folder); // Create if not exists
            var fileName = $"ev_{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}"; // Unique file name
            var rel = $"/uploads/{fileName}"; // Relative URL for DB
            var phys = Path.Combine(folder, fileName); // Physical path

            using (var fs = File.Create(phys)) // Save file
            {
                await dto.Image.CopyToAsync(fs); // Copy uploaded file to disk
            }
            // Create evidence record

            var e = new Evidence
            {
                CaseId = dto.CaseId,
                AddedByUserId = actorUserId,
                Type = EvidenceType.Image,
                FileUrl = rel,
                MimeType = dto.Image.ContentType,
                SizeBytes = dto.Image.Length,
                Remarks = dto.Remarks
            };

        
            // Add audit log
            _db.EvidenceAuditLogs.Add(new EvidenceAuditLog
            {
                Evidence = e,
                ActedByUserId = actorUserId,
                Action = "add",
                Details = "image"
            });

            // Save changes
            
            await _repo.SaveAsync();
            // Return result
            return (e.Id, "Image evidence uploaded");
        }
        // Get evidence by id
        public async Task<Evidence?> GetAsync(int id) => await _repo.GetReadOnlyAsync(id);

        public async Task<(byte[] bytes, string mime)?> GetImageAsync(int id, string rootPath)
        {
            // Get evidence
            var e = await _repo.GetReadOnlyAsync(id);
            // Validate
            if (e == null || e.Type != EvidenceType.Image || string.IsNullOrEmpty(e.FileUrl))
                return null;
            // Build physical path
            var phys = Path.Combine(rootPath, e.FileUrl.TrimStart('/'));
            // Check file exists
            if (!File.Exists(phys)) return null;
            // Read file bytes
            var bytes = await File.ReadAllBytesAsync(phys);
            // Return bytes and mime type
            return (bytes, e.MimeType!);
        }

        // Update evidence (only text content and remarks)
        public async Task<bool> UpdateAsync(int id, UpdateEvidenceDto dto, int actorUserId)
        {
            var e = await _db.Evidences.FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return false;

            if (e.Type == EvidenceType.Text && dto.TextContent != null)
            {
                //if (dto.TextContent == null) return false;
                e.TextContent = dto.TextContent;
            }

            // Update remarks if provided
            if (dto.Remarks != null)
            {
                e.Remarks = dto.Remarks;
            }

            e.UpdatedAt = DateTime.UtcNow;

            _db.EvidenceAuditLogs.Add(new EvidenceAuditLog
            {
                EvidenceId = e.Id,
                ActedByUserId = actorUserId,
                Action = "update",
                Details = "content/remarks"
            });

            await _repo.SaveAsync();
            return true;
        }

        // Soft delete evidence
        public async Task<bool> SoftDeleteAsync(int id, int actorUserId)
        {
            var e = await _db.Evidences.FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return false;

            e.IsSoftDeleted = true;
            e.UpdatedAt = DateTime.UtcNow;

            _db.EvidenceAuditLogs.Add(new EvidenceAuditLog
            {
                EvidenceId = id,
                ActedByUserId = actorUserId,
                Action = "soft_delete",
                Details = "soft"
            });

            await _db.SaveChangesAsync();
            return true;
        }

        // Hard delete evidence
        public async Task<bool> HardDeleteAsync(int id, int actorUserId, string rootPath)
        {
            var evidence = await _repo.GetReadOnlyAsync(id);
            if (evidence == null) return false;

            // Check permissions - only Admin and Investigator can hard delete
            var user = await _db.Users.FindAsync(actorUserId);
            if (user?.Role is (UserRole.Admin) or (UserRole.Investigator))
            {
                // Delete physical file if it's an image
                if (evidence.Type == EvidenceType.Image && !string.IsNullOrEmpty(evidence.FileUrl))
                {
                    var physicalPath = Path.Combine(rootPath, evidence.FileUrl.TrimStart('/'));
                    if (File.Exists(physicalPath))
                    {
                        File.Delete(physicalPath);
                    }
                }

                // Create audit log before deletion
                _db.EvidenceAuditLogs.Add(new EvidenceAuditLog
                {
                    EvidenceId = evidence.Id,
                    ActedByUserId = actorUserId,
                    Action = "hard_delete",
                    Details = "permanent_deletion",
                    ActedAt = DateTime.UtcNow
                });

                // Remove from database
                _db.Evidences.Remove(evidence);
                await _db.SaveChangesAsync();

                return true;
            }

            return false;
        }

        // get all evidence for a case
        public async Task<List<Evidence>> GetByCaseAsync(int caseId)
        {
            return await _db.Evidences
                .Where(e => e.CaseId == caseId && !e.IsSoftDeleted)
                .ToListAsync();
        }
    }
}
