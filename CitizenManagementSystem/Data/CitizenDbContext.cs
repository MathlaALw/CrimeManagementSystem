using CitizenManagementSystem.Models;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;

namespace CitizenManagementSystem.Data
{
    public class CitizenDbContext: DbContext
    {
       
            public CitizenDbContext(DbContextOptions<CitizenDbContext> options)
                : base(options)
            {
            }

            public DbSet<Citizen> Citizens => Set<Citizen>();

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<Citizen>(e =>
                {
                    e.HasKey(c => c.Id);
                    e.Property(c => c.FullName).IsRequired().HasMaxLength(150);
                    e.Property(c => c.Email).IsRequired().HasMaxLength(200);
                    e.Property(c => c.City).IsRequired().HasMaxLength(100);


                    e.HasIndex(c => c.Email).IsUnique();
                });
            }
        }

    }

