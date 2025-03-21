using System;
using System.Linq;
using System.Threading.Tasks;
using JobApplicationTracker.Domain.Entities;
using JobApplicationTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JobApplicationTracker.Infrastructure.Data
{
    /// <summary>
    /// Database initialization helper to seed initial data
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Initialize the database with sample data if empty
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="logger">Logger</param>
        public static async Task InitializeAsync(ApplicationDbContext context, ILogger logger)
        {
            try
            {
                // Create database if it doesn't exist
                await context.Database.EnsureCreatedAsync();

                // Seed data only if the job applications table is empty
                if (!await context.JobApplications.AnyAsync())
                {
                    logger.LogInformation("Seeding database with sample job applications...");
                    await SeedSampleDataAsync(context);
                    logger.LogInformation("Database seeding completed successfully.");
                }
                else
                {
                    logger.LogInformation("Database already contains data. Skipping seed operation.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }

        /// <summary>
        /// Seeds the database with sample job applications
        /// </summary>
        /// <param name="context">Database context</param>
        private static async Task SeedSampleDataAsync(ApplicationDbContext context)
        {
            var currentDate = DateTime.UtcNow;

            var sampleApplications = new JobApplication[]
            {
                new JobApplication
                {
                    CompanyName = "Microsoft",
                    Position = "Senior .NET Developer",
                    Status = ApplicationStatus.Applied,
                    DateApplied = currentDate.AddDays(-10),
                    ContactPerson = "John Smith",
                    ContactEmail = "john.smith@microsoft.com",
                    Notes = "Applied through company website. Received confirmation email.",
                    JobUrl = "https://careers.microsoft.com/jobs/123456",
                    SalaryRange = 120000,
                    CreatedAt = currentDate.AddDays(-10),
                    UpdatedAt = null
                },
                new JobApplication
                {
                    CompanyName = "Google",
                    Position = "Full Stack Engineer",
                    Status = ApplicationStatus.Interview,
                    DateApplied = currentDate.AddDays(-15),
                    ContactPerson = "Jane Doe",
                    ContactEmail = "jane.doe@google.com",
                    Notes = "First round technical interview scheduled for next week.",
                    JobUrl = "https://careers.google.com/jobs/124567",
                    SalaryRange = 140000,
                    CreatedAt = currentDate.AddDays(-15),
                    UpdatedAt = currentDate.AddDays(-12)
                },
                new JobApplication
                {
                    CompanyName = "Amazon",
                    Position = "Software Development Engineer II",
                    Status = ApplicationStatus.Rejected,
                    DateApplied = currentDate.AddDays(-30),
                    ContactPerson = "Recruiter",
                    ContactEmail = "recruiter@amazon.com",
                    Notes = "Rejected after phone screen. Will try again in 6 months.",
                    JobUrl = "https://amazon.jobs/en/jobs/987654",
                    SalaryRange = 135000,
                    CreatedAt = currentDate.AddDays(-30),
                    UpdatedAt = currentDate.AddDays(-20)
                },
                new JobApplication
                {
                    CompanyName = "Netflix",
                    Position = "Senior Frontend Developer",
                    Status = ApplicationStatus.Offer,
                    DateApplied = currentDate.AddDays(-45),
                    ContactPerson = "Technical Recruiter",
                    ContactEmail = "techrec@netflix.com",
                    Notes = "Offer received: $150K base + 15% bonus + stock options",
                    JobUrl = "https://jobs.netflix.com/jobs/235689",
                    SalaryRange = 150000,
                    CreatedAt = currentDate.AddDays(-45),
                    UpdatedAt = currentDate.AddDays(-5)
                },
                new JobApplication
                {
                    CompanyName = "Apple",
                    Position = "Software Engineer",
                    Status = ApplicationStatus.Applied,
                    DateApplied = currentDate.AddDays(-2),
                    ContactPerson = null,
                    ContactEmail = null,
                    Notes = "Applied through LinkedIn Easy Apply",
                    JobUrl = "https://jobs.apple.com/en-us/details/123456789",
                    SalaryRange = null,
                    CreatedAt = currentDate.AddDays(-2),
                    UpdatedAt = null
                }
            };

            await context.JobApplications.AddRangeAsync(sampleApplications);
            await context.SaveChangesAsync();
        }
    }
}