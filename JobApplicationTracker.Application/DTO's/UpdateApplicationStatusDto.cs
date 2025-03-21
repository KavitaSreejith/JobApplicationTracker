using JobApplicationTracker.Domain.Enums;

namespace JobApplicationTracker.Application.DTOs
{
    /// <summary>
    /// DTO for updating only the status of a job application
    /// </summary>
    public class UpdateApplicationStatusDto
    {
        /// <summary>
        /// New status to set for the application
        /// </summary>
        public ApplicationStatus Status { get; set; }
    }
}