using Crime_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Crime_Management_System.Data
{
    public class CrimeDbContext : DbContext
    {
        public CrimeDbContext(DbContextOptions<CrimeDbContext> options) : base(options) { }

        // DbSets for each entity
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Case> Cases { get; set; } = null!;
        public DbSet<Participant> Participants { get; set; } = null!;
        public DbSet<CaseParticipant> CaseParticipants { get; set; } = null!;
        public DbSet<CrimeReport> CrimeReports { get; set; } = null!;
        public DbSet<CaseReport> CaseReports { get; set; } = null!;
        public DbSet<CaseAssignee> CaseAssignees { get; set; } = null!;
        public DbSet<Evidence> Evidences { get; set; } = null!;
        public DbSet<EvidenceAuditLog> EvidenceAuditLogs { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder b)
        {
            // Configure unique constraints
            b.Entity<User>().HasIndex(u => u.Username).IsUnique();
            b.Entity<User>().HasIndex(u => u.Email).IsUnique();
            b.Entity<Case>().HasIndex(c => c.CaseNumber).IsUnique();

            // Configure composite unique constraints
            b.Entity<CaseReport>().HasIndex(x => new { x.CaseId, x.ReportId }).IsUnique();
            b.Entity<CaseAssignee>().HasIndex(x => new { x.CaseId, x.UserId }).IsUnique();


            // CaseAssignee: Case (principal) -> CaseAssignees (dependent)
            b.Entity<CaseAssignee>(e =>
            {
                e.HasOne(x => x.Case)
                 .WithMany(c => c.CaseAssignees)
                 .HasForeignKey(x => x.CaseId)
                 .OnDelete(DeleteBehavior.Cascade);         // keep cascade from Case

                e.HasOne(x => x.User)
                 .WithMany(u => u.CaseAssignees)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.NoAction);        // break cascade from User
            });

            // CaseReport: junction between Case and CrimeReport
            b.Entity<CaseReport>(e =>
            {
                e.HasOne(x => x.Case)
                 .WithMany(c => c.CaseReports)
                 .HasForeignKey(x => x.CaseId)
                 .OnDelete(DeleteBehavior.Cascade);         // deleting case deletes its links

                e.HasOne(x => x.Report)
                 .WithMany(r => r.CaseReports)
                 .HasForeignKey(x => x.ReportId)
                 .OnDelete(DeleteBehavior.NoAction);        // reference entity -> no cascade
            });

            // CaseParticipant: junction between Case and Participant
            b.Entity<CaseParticipant>(e =>
            {
                e.HasOne(x => x.Case)
                 .WithMany(c => c.CaseParticipants)
                 .HasForeignKey(x => x.CaseId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Participant)
                 .WithMany(p => p.CaseParticipants)
                 .HasForeignKey(x => x.ParticipantId)
                 .OnDelete(DeleteBehavior.NoAction);        // participant is a reference entity
            });

            // Evidence: belongs to Case; optionally uploaded by User (if you have UploaderUserId)
            b.Entity<Evidence>(e =>
            {
                e.HasOne(x => x.Case)
                 .WithMany(c => c.Evidences)
                 .HasForeignKey(x => x.CaseId)
                 .OnDelete(DeleteBehavior.Cascade);

                // If your model has: public int? UploaderUserId; public User Uploader { get; set; }
                // Uncomment if applicable:
                // e.HasOne(x => x.Uploader)
                //  .WithMany(u => u.UploadedEvidences)
                //  .HasForeignKey(x => x.UploaderUserId)
                //  .OnDelete(DeleteBehavior.NoAction);
            });

            // EvidenceAuditLog: belongs to Evidence; optionally linked to User (actor)
            b.Entity<EvidenceAuditLog>(e =>
            {
                e.HasOne(x => x.Evidence)
                 .WithMany(ev => ev.AuditLogs)
                 .HasForeignKey(x => x.EvidenceId)
                 .OnDelete(DeleteBehavior.Cascade);         // delete evidence -> delete its logs

                // If model has: public int? UserId; public User User { get; set; }
                // We do NOT cascade from User:
                e.HasOne(x => x.User)
                 .WithMany(u => u.EvidenceAuditLogs)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.NoAction);
            });

            // Evidence CHECK constraint : either text OR image
            b.Entity<Evidence>(entity =>
            {
                
                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Evidence_TextOrImage",
                        "(Type = 0 AND TextContent IS NOT NULL AND FileUrl IS NULL) OR " +
                        "(Type = 1 AND FileUrl IS NOT NULL AND TextContent IS NULL)");
                });
            });
        }


    }
    
}
