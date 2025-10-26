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


        }
    }
}
