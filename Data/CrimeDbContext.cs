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

            // Users 
            b.Entity<User>()
                .HasIndex(u => u.Username).IsUnique();
            b.Entity<User>()
                .HasIndex(u => u.Email).IsUnique();

            // Cases 
            b.Entity<Case>()
                .HasIndex(c => c.CaseNumber).IsUnique();

            // Case.CreatedByUser 
            b.Entity<Case>()
                .HasOne(c => c.CreatedByUser)
                .WithMany(u => u.CreatedCases)
                .HasForeignKey(c => c.CreatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

          
            // ===== CaseAssignee =====
            b.Entity<CaseAssignee>()
                .HasIndex(x => new { x.CaseId, x.UserId })  
                .IsUnique();

            b.Entity<CaseAssignee>()
                .HasOne(x => x.Case)
                .WithMany(c => c.CaseAssignees)
                .HasForeignKey(x => x.CaseId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<CaseAssignee>()
                .HasOne(x => x.User)                      
                .WithMany(u => u.CaseAssignees)
                .HasForeignKey(x => x.UserId)                
                .OnDelete(DeleteBehavior.NoAction);

            // ===== CaseParticipant =====
            b.Entity<CaseParticipant>()
                .HasIndex(x => new { x.CaseId, x.ParticipantId })
                .IsUnique();

            b.Entity<CaseParticipant>()
                .HasOne(x => x.Case)
                .WithMany(c => c.CaseParticipants)
                .HasForeignKey(x => x.CaseId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<CaseParticipant>()
                .HasOne(x => x.Participant)
                .WithMany(p => p.CaseParticipants)   
                .HasForeignKey(x => x.ParticipantId)
                .OnDelete(DeleteBehavior.NoAction);

            b.Entity<CaseParticipant>()
                 .HasOne(x => x.AddedByUser)
                 .WithMany()                         
                 .HasForeignKey(x => x.AddedByUserId)
                 .OnDelete(DeleteBehavior.NoAction);

            // ===== CrimeReport & CaseReport =====
            b.Entity<CrimeReport>()
                .HasOne(cr => cr.ReportedByUser)             // nullable (citizen allowed)
                .WithMany(u => u.CrimeReports)               // your current User.cs has CrimeReports
                .HasForeignKey(cr => cr.ReportedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Evidence
            // Evidence belongs to Case (required) and AddedByUser (required)
            b.Entity<Evidence>()
                .HasOne(e => e.Case)
                .WithMany(c => c.Evidences)
                .HasForeignKey(e => e.CaseId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<Evidence>()
                .HasOne(e => e.AddedByUser)
                .WithMany(u => u.AddedEvidences)
                .HasForeignKey(e => e.AddedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Check constraint matches your model (Type=Text -> TextContent required; Type=Image -> FileUrl required)
            b.Entity<Evidence>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_Evidence_TypeAndContent",
                    "(Type IN (0,1)) AND ((Type = 0 AND TextContent IS NOT NULL) OR (Type = 1 AND FileUrl IS NOT NULL))"
                ));
            // Evidence fields confirmed in model:contentReference[oaicite:5]{index=5} and enum:contentReference[oaicite:6]{index=6}

            //EvidenceAuditLog
            b.Entity<EvidenceAuditLog>()
                .HasOne(ea => ea.Evidence)
                .WithMany(e => e.AuditLogs)
                .HasForeignKey(ea => ea.EvidenceId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<EvidenceAuditLog>()
                .HasOne(ea => ea.ActedByUser)
                .WithMany(u => u.EvidenceAuditLogs)
                .HasForeignKey(ea => ea.ActedByUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }


    }
    
}
