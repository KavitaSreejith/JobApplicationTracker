using JobApplicationTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Infrastructure.Data
{
    /// <summary>
    /// EF Core database context for the application
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// DbSet for job applications
        /// </summary>
        public DbSet<JobApplication> JobApplications { get; set; } = null!;

        /// <summary>
        /// Configure entity mappings and relationships
        /// </summary>
        /// <param name="modelBuilder">The model builder</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure JobApplication entity
            modelBuilder.Entity<JobApplication>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Position).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.DateApplied).IsRequired();
                entity.Property(e => e.ContactPerson).HasMaxLength(100);
                entity.Property(e => e.ContactEmail).HasMaxLength(100);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.JobUrl).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).IsRequired();
            });
        }
    }
}