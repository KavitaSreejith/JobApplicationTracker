using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JobApplicationTracker.Domain.Entities;
using JobApplicationTracker.Domain.Enums;

namespace JobApplicationTracker.Application.Interfaces
{
    /// <summary>
    /// Repository interface for job application data access
    /// </summary>
    public interface IJobApplicationRepository
    {
        /// <summary>
        /// Gets all job applications
        /// </summary>
        /// <returns>List of all job applications</returns>
        Task<IEnumerable<JobApplication>> GetAllAsync();

        /// <summary>
        /// Gets job applications with pagination and filtering options
        /// </summary>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="status">Optional filter by status</param>
        /// <param name="searchTerm">Optional search term for company or position</param>
        /// <returns>Filtered and paginated job applications</returns>
        Task<(IEnumerable<JobApplication> Applications, int TotalCount)> GetApplicationsAsync(
            int pageNumber,
            int pageSize,
            ApplicationStatus? status = null,
            string? searchTerm = null);

        /// <summary>
        /// Gets a job application by ID
        /// </summary>
        /// <param name="id">Job application ID</param>
        /// <returns>The job application if found, null otherwise</returns>
        Task<JobApplication?> GetByIdAsync(int id);

        /// <summary>
        /// Creates a new job application
        /// </summary>
        /// <param name="jobApplication">Job application to create</param>
        /// <returns>The created job application</returns>
        Task<JobApplication> CreateAsync(JobApplication jobApplication);

        /// <summary>
        /// Updates an existing job application
        /// </summary>
        /// <param name="jobApplication">Job application with updated values</param>
        /// <returns>The updated job application</returns>
        Task<JobApplication> UpdateAsync(JobApplication jobApplication);

        /// <summary>
        /// Deletes a job application
        /// </summary>
        /// <param name="id">ID of the job application to delete</param>
        /// <returns>True if deleted successfully, false if not found</returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Checks if a job application exists
        /// </summary>
        /// <param name="id">ID to check</param>
        /// <returns>True if exists, false otherwise</returns>
        Task<bool> ExistsAsync(int id);

        /// <summary>
        /// Gets counts of applications by status
        /// </summary>
        /// <returns>Dictionary with counts for each status</returns>
        Task<Dictionary<ApplicationStatus, int>> GetApplicationCountsByStatusAsync();
    }
}