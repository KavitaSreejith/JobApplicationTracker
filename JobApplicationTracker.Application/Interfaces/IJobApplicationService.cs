using System.Threading.Tasks;
using JobApplicationTracker.Application.DTOs;
using JobApplicationTracker.Domain.Enums;

namespace JobApplicationTracker.Application.Interfaces
{
    /// <summary>
    /// Service interface for job application business logic
    /// </summary>
    public interface IJobApplicationService
    {
        /// <summary>
        /// Gets all job applications with optional filtering and pagination
        /// </summary>
        /// <param name="filter">Pagination and filter options</param>
        /// <returns>Paginated response with job applications</returns>
        Task<PaginatedResponseDto<JobApplicationDto>> GetAllAsync(PaginationFilterDto filter);

        /// <summary>
        /// Gets a job application by ID
        /// </summary>
        /// <param name="id">The ID of the job application</param>
        /// <returns>The job application if found, null otherwise</returns>
        Task<JobApplicationDto?> GetByIdAsync(int id);

        /// <summary>
        /// Creates a new job application
        /// </summary>
        /// <param name="createDto">Data for creating the job application</param>
        /// <returns>The created job application</returns>
        Task<JobApplicationDto> CreateAsync(CreateJobApplicationDto createDto);

        /// <summary>
        /// Updates an existing job application
        /// </summary>
        /// <param name="id">ID of the job application to update</param>
        /// <param name="updateDto">Updated job application data</param>
        /// <returns>The updated job application</returns>
        Task<JobApplicationDto?> UpdateAsync(int id, UpdateJobApplicationDto updateDto);

        /// <summary>
        /// Updates only the status of a job application
        /// </summary>
        /// <param name="id">ID of the job application</param>
        /// <param name="statusDto">New status data</param>
        /// <returns>The updated job application</returns>
        Task<JobApplicationDto?> UpdateStatusAsync(int id, UpdateApplicationStatusDto statusDto);

        /// <summary>
        /// Deletes a job application
        /// </summary>
        /// <param name="id">ID of the job application to delete</param>
        /// <returns>True if deleted successfully, false if not found</returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Gets application statistics
        /// </summary>
        /// <returns>Application statistics</returns>
        Task<JobApplicationStatisticsDto> GetStatisticsAsync();
    }
}