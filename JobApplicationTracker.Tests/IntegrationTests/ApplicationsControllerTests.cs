using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using JobApplicationTracker.Application.DTOs;
using JobApplicationTracker.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace JobApplicationTracker.Tests.IntegrationTests
{
    public class ApplicationsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApplicationsControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        [Fact]
        public async Task GetApplications_ReturnsSuccessAndCorrectContentType()
        {
            // Act
            var response = await _client.GetAsync("/api/applications");

            // Assert
            response.EnsureSuccessStatusCode();
            response.Content.Headers.ContentType!.ToString().Should().Be("application/json; charset=utf-8");
        }

        [Fact]
        public async Task GetApplications_WithPagination_ReturnsPaginatedResults()
        {
            // Act
            var response = await _client.GetAsync("/api/applications?pageNumber=1&pageSize=2");

            // Assert
            response.EnsureSuccessStatusCode();

            // Verify pagination headers are included
            response.Headers.Contains("X-Pagination").Should().BeTrue();
            var paginationHeader = response.Headers.GetValues("X-Pagination").First();
            var pagination = JsonSerializer.Deserialize<Dictionary<string, object>>(paginationHeader, _jsonOptions);
            pagination.Should().ContainKey("totalCount");
            pagination.Should().ContainKey("pageNumber");
            pagination.Should().ContainKey("pageSize");

            var content = await response.Content.ReadFromJsonAsync<PaginatedResponseDto<JobApplicationDto>>(_jsonOptions);
            content.Should().NotBeNull();
            content!.Items.Should().NotBeNull();
            content.PageSize.Should().Be(2);
            content.PageNumber.Should().Be(1);
        }

        [Fact]
        public async Task GetApplicationById_WithExistingId_ReturnsApplication()
        {
            // Arrange - First create an application to get its ID
            var createDto = new CreateJobApplicationDto
            {
                CompanyName = "Test Company",
                Position = "Test Position",
                Status = ApplicationStatus.Applied,
                DateApplied = DateTime.UtcNow
            };

            var createResponse = await _client.PostAsJsonAsync("/api/applications", createDto);
            createResponse.EnsureSuccessStatusCode();

            var createdApp = await createResponse.Content.ReadFromJsonAsync<JobApplicationDto>(_jsonOptions);
            var id = createdApp!.Id;

            // Act
            var response = await _client.GetAsync($"/api/applications/{id}");

            // Assert
            response.EnsureSuccessStatusCode();
            var application = await response.Content.ReadFromJsonAsync<JobApplicationDto>(_jsonOptions);

            application.Should().NotBeNull();
            application!.Id.Should().Be(id);
            application.CompanyName.Should().Be("Test Company");
            application.Position.Should().Be("Test Position");
        }

        [Fact]
        public async Task GetApplicationById_WithNonExistingId_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/applications/99999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateApplication_ValidData_ReturnsCreatedAndNewApplication()
        {
            // Arrange
            var createDto = new CreateJobApplicationDto
            {
                CompanyName = "New Company",
                Position = "New Position",
                Status = ApplicationStatus.Applied,
                DateApplied = DateTime.UtcNow,
                Notes = "Test notes"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/applications", createDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdApp = await response.Content.ReadFromJsonAsync<JobApplicationDto>(_jsonOptions);

            createdApp.Should().NotBeNull();
            createdApp!.Id.Should().BeGreaterThan(0);
            createdApp.CompanyName.Should().Be("New Company");
            createdApp.Position.Should().Be("New Position");
            createdApp.Notes.Should().Be("Test notes");

            // Verify the Location header
            response.Headers.Location.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateApplication_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var invalidDto = new
            {
                // Missing required fields CompanyName and Position
                Status = 0,
                DateApplied = DateTime.UtcNow
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/applications", invalidDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateApplication_ExistingIdAndValidData_ReturnsNoContent()
        {
            // Arrange - First create an application to update
            var createDto = new CreateJobApplicationDto
            {
                CompanyName = "Company To Update",
                Position = "Position To Update",
                Status = ApplicationStatus.Applied,
                DateApplied = DateTime.UtcNow
            };

            var createResponse = await _client.PostAsJsonAsync("/api/applications", createDto);
            createResponse.EnsureSuccessStatusCode();

            var createdApp = await createResponse.Content.ReadFromJsonAsync<JobApplicationDto>(_jsonOptions);
            var id = createdApp!.Id;

            // Update data
            var updateDto = new UpdateJobApplicationDto
            {
                CompanyName = "Updated Company",
                Position = "Updated Position",
                Status = ApplicationStatus.Interview,
                DateApplied = DateTime.UtcNow.AddDays(-1)
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/applications/{id}", updateDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verify the application was actually updated
            var getResponse = await _client.GetAsync($"/api/applications/{id}");
            getResponse.EnsureSuccessStatusCode();

            var updatedApp = await getResponse.Content.ReadFromJsonAsync<JobApplicationDto>(_jsonOptions);
            updatedApp.Should().NotBeNull();
            updatedApp!.CompanyName.Should().Be("Updated Company");
            updatedApp.Position.Should().Be("Updated Position");
            updatedApp.Status.Should().Be(ApplicationStatus.Interview);
        }

        [Fact]
        public async Task UpdateApplicationStatus_ExistingIdAndValidStatus_ReturnsNoContent()
        {
            // Arrange - First create an application to update its status
            var createDto = new CreateJobApplicationDto
            {
                CompanyName = "Status Test Company",
                Position = "Status Test Position",
                Status = ApplicationStatus.Applied,
                DateApplied = DateTime.UtcNow
            };

            var createResponse = await _client.PostAsJsonAsync("/api/applications", createDto);
            createResponse.EnsureSuccessStatusCode();

            var createdApp = await createResponse.Content.ReadFromJsonAsync<JobApplicationDto>(_jsonOptions);
            var id = createdApp!.Id;

            // Status update data
            var statusDto = new UpdateApplicationStatusDto
            {
                Status = ApplicationStatus.Offer
            };

            // Act
            var response = await _client.PatchAsJsonAsync($"/api/applications/{id}/status", statusDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verify the status was actually updated
            var getResponse = await _client.GetAsync($"/api/applications/{id}");
            getResponse.EnsureSuccessStatusCode();

            var updatedApp = await getResponse.Content.ReadFromJsonAsync<JobApplicationDto>(_jsonOptions);
            updatedApp.Should().NotBeNull();
            updatedApp!.Status.Should().Be(ApplicationStatus.Offer);
        }

        [Fact]
        public async Task DeleteApplication_ExistingId_ReturnsNoContent()
        {
            // Arrange - First create an application to delete
            var createDto = new CreateJobApplicationDto
            {
                CompanyName = "Company To Delete",
                Position = "Position To Delete",
                Status = ApplicationStatus.Applied,
                DateApplied = DateTime.UtcNow
            };

            var createResponse = await _client.PostAsJsonAsync("/api/applications", createDto);
            createResponse.EnsureSuccessStatusCode();

            var createdApp = await createResponse.Content.ReadFromJsonAsync<JobApplicationDto>(_jsonOptions);
            var id = createdApp!.Id;

            // Act
            var response = await _client.DeleteAsync($"/api/applications/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verify the application was actually deleted
            var getResponse = await _client.GetAsync($"/api/applications/{id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetStatistics_ReturnsSuccess()
        {
            // Act
            var response = await _client.GetAsync("/api/applications/statistics");

            // Assert
            response.EnsureSuccessStatusCode();

            var statistics = await response.Content.ReadFromJsonAsync<JobApplicationStatisticsDto>(_jsonOptions);
            statistics.Should().NotBeNull();
            statistics!.TotalCount.Should().BeGreaterThanOrEqualTo(0);
        }
    }

    // Extension method for PatchAsJsonAsync as it's not included in HttpClient by default
    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> PatchAsJsonAsync<T>(this HttpClient client, string requestUri, T value)
        {
            var content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
            return client.PatchAsync(requestUri, content);
        }
    }
}