using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using JobApplicationTracker.Application.DTOs;
using JobApplicationTracker.Application.Interfaces;
using JobApplicationTracker.Application.Mappings;
using JobApplicationTracker.Application.Services;
using JobApplicationTracker.Domain.Entities;
using JobApplicationTracker.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace JobApplicationTracker.Tests.UnitTests
{
    public class JobApplicationServiceTests
    {
        private readonly Mock<IJobApplicationRepository> _mockRepository;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<JobApplicationService>> _mockLogger;
        private readonly JobApplicationService _service;

        public JobApplicationServiceTests()
        {
            _mockRepository = new Mock<IJobApplicationRepository>();

            // Create actual mapper instance with real profiles
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            _mapper = mappingConfig.CreateMapper();

            _mockLogger = new Mock<ILogger<JobApplicationService>>();

            _service = new JobApplicationService(_mockRepository.Object, _mapper, _mockLogger.Object);
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ReturnsJobApplication()
        {
            // Arrange
            var jobApplicationId = 1;
            var jobApplication = new JobApplication
            {
                Id = jobApplicationId,
                CompanyName = "Test Company",
                Position = "Software Developer",
                Status = ApplicationStatus.Applied,
                DateApplied = DateTime.UtcNow.AddDays(-5),
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(jobApplicationId))
                .ReturnsAsync(jobApplication);

            // Act
            var result = await _service.GetByIdAsync(jobApplicationId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(jobApplicationId);
            result.CompanyName.Should().Be("Test Company");
            result.Position.Should().Be("Software Developer");
            result.Status.Should().Be(ApplicationStatus.Applied);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ReturnsNull()
        {
            // Arrange
            var nonExistingId = 999;
            _mockRepository.Setup(repo => repo.GetByIdAsync(nonExistingId))
                .ReturnsAsync((JobApplication)null!);

            // Act
            var result = await _service.GetByIdAsync(nonExistingId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_ValidData_ReturnsCreatedApplication()
        {
            // Arrange
            var createDto = new CreateJobApplicationDto
            {
                CompanyName = "New Company",
                Position = "Senior Developer",
                Status = ApplicationStatus.Applied,
                DateApplied = DateTime.UtcNow,
                Notes = "Test notes"
            };

            _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<JobApplication>()))
                .ReturnsAsync((JobApplication jobApp) =>
                {
                    jobApp.Id = 1; // Simulate ID assignment by database
                    return jobApp;
                });

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.CompanyName.Should().Be("New Company");
            result.Position.Should().Be("Senior Developer");
            result.Status.Should().Be(ApplicationStatus.Applied);
            result.Notes.Should().Be("Test notes");

            _mockRepository.Verify(repo => repo.CreateAsync(It.IsAny<JobApplication>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ExistingApplication_ReturnsUpdatedApplication()
        {
            // Arrange
            var id = 1;
            var existingApplication = new JobApplication
            {
                Id = id,
                CompanyName = "Old Company",
                Position = "Junior Developer",
                Status = ApplicationStatus.Applied,
                DateApplied = DateTime.UtcNow.AddDays(-10),
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            };

            var updateDto = new UpdateJobApplicationDto
            {
                CompanyName = "Updated Company",
                Position = "Updated Position",
                Status = ApplicationStatus.Interview,
                DateApplied = DateTime.UtcNow.AddDays(-9),
                Notes = "Updated notes"
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(existingApplication);

            _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<JobApplication>()))
                .ReturnsAsync((JobApplication jobApp) => jobApp);

            // Act
            var result = await _service.UpdateAsync(id, updateDto);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(id);
            result.CompanyName.Should().Be("Updated Company");
            result.Position.Should().Be("Updated Position");
            result.Status.Should().Be(ApplicationStatus.Interview);
            result.Notes.Should().Be("Updated notes");

            _mockRepository.Verify(repo => repo.GetByIdAsync(id), Times.Once);
            _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<JobApplication>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_NonExistingApplication_ReturnsNull()
        {
            // Arrange
            var nonExistingId = 999;
            var updateDto = new UpdateJobApplicationDto
            {
                CompanyName = "Updated Company",
                Position = "Updated Position",
                Status = ApplicationStatus.Interview,
                DateApplied = DateTime.UtcNow
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(nonExistingId))
                .ReturnsAsync((JobApplication)null!);

            // Act
            var result = await _service.UpdateAsync(nonExistingId, updateDto);

            // Assert
            result.Should().BeNull();
            _mockRepository.Verify(repo => repo.GetByIdAsync(nonExistingId), Times.Once);
            _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<JobApplication>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ExistingApplication_ReturnsTrue()
        {
            // Arrange
            var id = 1;
            _mockRepository.Setup(repo => repo.DeleteAsync(id))
                .ReturnsAsync(true);

            // Act
            var result = await _service.DeleteAsync(id);

            // Assert
            result.Should().BeTrue();
            _mockRepository.Verify(repo => repo.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_NonExistingApplication_ReturnsFalse()
        {
            // Arrange
            var nonExistingId = 999;
            _mockRepository.Setup(repo => repo.DeleteAsync(nonExistingId))
                .ReturnsAsync(false);

            // Act
            var result = await _service.DeleteAsync(nonExistingId);

            // Assert
            result.Should().BeFalse();
            _mockRepository.Verify(repo => repo.DeleteAsync(nonExistingId), Times.Once);
        }

        [Fact]
        public async Task GetStatisticsAsync_ReturnsSummaryStatistics()
        {
            // Arrange
            var statusCounts = new Dictionary<ApplicationStatus, int>
            {
                { ApplicationStatus.Applied, 10 },
                { ApplicationStatus.Interview, 5 },
                { ApplicationStatus.Offer, 2 },
                { ApplicationStatus.Rejected, 3 }
            };

            _mockRepository.Setup(repo => repo.GetApplicationCountsByStatusAsync())
                .ReturnsAsync(statusCounts);

            // Act
            var result = await _service.GetStatisticsAsync();

            // Assert
            result.Should().NotBeNull();
            result.TotalCount.Should().Be(20);
            result.AppliedCount.Should().Be(10);
            result.InterviewCount.Should().Be(5);
            result.OfferCount.Should().Be(2);
            result.RejectedCount.Should().Be(3);
            result.SuccessRate.Should().Be(10.00m); // (2/20) * 100
            result.InterviewRate.Should().Be(33.33m); // (5/15) * 100

            _mockRepository.Verify(repo => repo.GetApplicationCountsByStatusAsync(), Times.Once);
        }
    }
}