using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using JobApplicationTracker.Domain.Entities;
using JobApplicationTracker.Domain.Enums;
using JobApplicationTracker.Infrastructure.Data;
using JobApplicationTracker.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace JobApplicationTracker.Tests.UnitTests
{
    public class JobApplicationRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly Mock<ILogger<JobApplicationRepository>> _mockLogger;

        public JobApplicationRepositoryTests()
        {
            // Use in-memory database for testing
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _mockLogger = new Mock<ILogger<JobApplicationRepository>>();

            // Initialize the database with test data
            using var dbContext = new ApplicationDbContext(_dbContextOptions);
            dbContext.Database.EnsureCreated();

            if (!dbContext.JobApplications.Any())
            {
                dbContext.JobApplications.AddRange(GetSampleJobApplications());
                dbContext.SaveChanges();
            }
        }

        private static JobApplication[] GetSampleJobApplications()
        {
            return new JobApplication[]
            {
                new JobApplication
                {
                    Id = 1,
                    CompanyName = "Company A",
                    Position = "Software Engineer",
                    Status = ApplicationStatus.Applied,
                    DateApplied = DateTime.UtcNow.AddDays(-5),
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new JobApplication
                {
                    Id = 2,
                    CompanyName = "Company B",
                    Position = "Senior Developer",
                    Status = ApplicationStatus.Interview,
                    DateApplied = DateTime.UtcNow.AddDays(-10),
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    UpdatedAt = DateTime.UtcNow.AddDays(-8)
                },
                new JobApplication
                {
                    Id = 3,
                    CompanyName = "Company C",
                    Position = "Full Stack Developer",
                    Status = ApplicationStatus.Rejected,
                    DateApplied = DateTime.UtcNow.AddDays(-15),
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    UpdatedAt = DateTime.UtcNow.AddDays(-7)
                },
                new JobApplication
                {
                    Id = 4,
                    CompanyName = "Company D",
                    Position = "Tech Lead",
                    Status = ApplicationStatus.Offer,
                    DateApplied = DateTime.UtcNow.AddDays(-20),
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new JobApplication
                {
                    Id = 5,
                    CompanyName = "Company E",
                    Position = "Backend Developer",
                    Status = ApplicationStatus.Applied,
                    DateApplied = DateTime.UtcNow.AddDays(-3),
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                }
            };
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllApplicationsOrderedByDateDescending()
        {
            // Arrange
            using var dbContext = new ApplicationDbContext(_dbContextOptions);
            var repository = new JobApplicationRepository(dbContext, _mockLogger.Object);

            // Act
            var applications = await repository.GetAllAsync();

            // Assert
            applications.Should().NotBeNull();
            applications.Should().HaveCount(5);
            applications.First().CompanyName.Should().Be("Company A"); // Most recent application
        }

        [Fact]
        public async Task GetApplicationsAsync_WithPageSizeAndNumber_ReturnsPaginatedResults()
        {
            // Arrange
            using var dbContext = new ApplicationDbContext(_dbContextOptions);
            var repository = new JobApplicationRepository(dbContext, _mockLogger.Object);

            // Act
            var (applications, totalCount) = await repository.GetApplicationsAsync(pageNumber: 1, pageSize: 2);

            // Assert
            applications.Should().NotBeNull();
            applications.Should().HaveCount(2);
            totalCount.Should().Be(5);
        }

        [Fact]
        public async Task GetApplicationsAsync_WithStatusFilter_ReturnsFilteredResults()
        {
            // Arrange
            using var dbContext = new ApplicationDbContext(_dbContextOptions);
            var repository = new JobApplicationRepository(dbContext, _mockLogger.Object);

            // Act
            var (applications, totalCount) = await repository.GetApplicationsAsync(
                pageNumber: 1,
                pageSize: 10,
                status: ApplicationStatus.Applied);

            // Assert
            applications.Should().NotBeNull();
            applications.Should().HaveCount(2);
            applications.All(a => a.Status == ApplicationStatus.Applied).Should().BeTrue();
            totalCount.Should().Be(2);
        }

        [Fact]
        public async Task GetApplicationsAsync_WithSearchTerm_ReturnsFilteredResults()
        {
            // Arrange
            using var dbContext = new ApplicationDbContext(_dbContextOptions);
            var repository = new JobApplicationRepository(dbContext, _mockLogger.Object);

            // Act
            var (applications, totalCount) = await repository.GetApplicationsAsync(
                pageNumber: 1,
                pageSize: 10,
                searchTerm: "senior");

            // Assert
            applications.Should().NotBeNull();
            applications.Should().HaveCount(1);
            applications.First().Position.Should().Contain("Senior");
            totalCount.Should().Be(1);
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ReturnsApplication()
        {
            // Arrange
            using var dbContext = new ApplicationDbContext(_dbContextOptions);
            var repository = new JobApplicationRepository(dbContext, _mockLogger.Object);

            // Act
            var application = await repository.GetByIdAsync(1);

            // Assert
            application.Should().NotBeNull();
            application!.Id.Should().Be(1);
            application.CompanyName.Should().Be("Company A");
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ReturnsNull()
        {
            // Arrange
            using var dbContext = new ApplicationDbContext(_dbContextOptions);
            var repository = new JobApplicationRepository(dbContext, _mockLogger.Object);

            // Act
            var application = await repository.GetByIdAsync(999);

            // Assert
            application.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_AddsNewApplication()
        {
            // Arrange
            using var dbContext = new ApplicationDbContext(_dbContextOptions);
            var repository = new JobApplicationRepository(dbContext, _mockLogger.Object);

            var newApplication = new JobApplication
            {
                CompanyName = "New Company",
                Position = "Developer",
                Status = ApplicationStatus.Applied,
                DateApplied = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var createdApplication = await repository.CreateAsync(newApplication);

            // Assert
            createdApplication.Should().NotBeNull();
            createdApplication.Id.Should().BeGreaterThan(0);
            createdApplication.CompanyName.Should().Be("New Company");

            // Verify the application was added to the database
            var applicationInDb = await dbContext.JobApplications.FindAsync(createdApplication.Id);
            applicationInDb.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateAsync_WithExistingApplication_UpdatesApplication()
        {
            // Arrange
            using var dbContext = new ApplicationDbContext(_dbContextOptions);
            var repository = new JobApplicationRepository(dbContext, _mockLogger.Object);

            var existingApplication = await dbContext.JobApplications.FindAsync(1);
            existingApplication!.CompanyName = "Updated Company";
            existingApplication.Status = ApplicationStatus.Interview;

            // Act
            var updatedApplication = await repository.UpdateAsync(existingApplication);

            // Assert
            updatedApplication.Should().NotBeNull();
            updatedApplication.CompanyName.Should().Be("Updated Company");
            updatedApplication.Status.Should().Be(ApplicationStatus.Interview);
            updatedApplication.UpdatedAt.Should().NotBeNull();

            // Verify the application was updated in the database
            var applicationInDb = await dbContext.JobApplications.FindAsync(1);
            applicationInDb!.CompanyName.Should().Be("Updated Company");
            applicationInDb.Status.Should().Be(ApplicationStatus.Interview);
        }

        [Fact]
        public async Task DeleteAsync_WithExistingId_RemovesApplicationAndReturnsTrue()
        {
            // Arrange
            using var dbContext = new ApplicationDbContext(_dbContextOptions);
            var repository = new JobApplicationRepository(dbContext, _mockLogger.Object);

            // Act
            var result = await repository.DeleteAsync(1);

            // Assert
            result.Should().BeTrue();

            // Verify the application was removed from the database
            var applicationInDb = await dbContext.JobApplications.FindAsync(1);
            applicationInDb.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistingId_ReturnsFalse()
        {
            // Arrange
            using var dbContext = new ApplicationDbContext(_dbContextOptions);
            var repository = new JobApplicationRepository(dbContext, _mockLogger.Object);

            // Act
            var result = await repository.DeleteAsync(999);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ExistsAsync_WithExistingId_ReturnsTrue()
        {
            // Arrange
            using var dbContext = new ApplicationDbContext(_dbContextOptions);
            var repository = new JobApplicationRepository(dbContext, _mockLogger.Object);

            // Act
            var exists = await repository.ExistsAsync(1);

            // Assert
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_WithNonExistingId_ReturnsFalse()
        {
            // Arrange
            using var dbContext = new ApplicationDbContext(_dbContextOptions);
            var repository = new JobApplicationRepository(dbContext, _mockLogger.Object);

            // Act
            var exists = await repository.ExistsAsync(999);

            // Assert
            exists.Should().BeFalse();
        }

        [Fact]
        public async Task GetApplicationCountsByStatusAsync_ReturnsDictionaryWithCounts()
        {
            // Arrange
            using var dbContext = new ApplicationDbContext(_dbContextOptions);
            var repository = new JobApplicationRepository(dbContext, _mockLogger.Object);

            // Act
            var counts = await repository.GetApplicationCountsByStatusAsync();

            // Assert
            counts.Should().NotBeNull();
            counts.Should().ContainKey(ApplicationStatus.Applied);
            counts.Should().ContainKey(ApplicationStatus.Interview);
            counts.Should().ContainKey(ApplicationStatus.Offer);
            counts.Should().ContainKey(ApplicationStatus.Rejected);

            counts[ApplicationStatus.Applied].Should().Be(2);
            counts[ApplicationStatus.Interview].Should().Be(1);
            counts[ApplicationStatus.Offer].Should().Be(1);
            counts[ApplicationStatus.Rejected].Should().Be(1);
        }
    }
}