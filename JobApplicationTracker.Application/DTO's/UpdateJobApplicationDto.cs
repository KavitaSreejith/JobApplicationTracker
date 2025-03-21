using System;
using JobApplicationTracker.Domain.Enums;

namespace JobApplicationTracker.Application.DTOs
{
    /// <summary>
    /// DTO for updating an existing job application
    /// </summary>
    public class UpdateJobApplicationDto
    {
        /// <summary>
        /// Name of the company
        /// </summary>
        public string CompanyName { get; set; } = string.Empty;

        /// <summary>
        /// Job position or role applied for
        /// </summary>
        public string Position { get; set; } = string.Empty;

        /// <summary>
        /// Current status of the application
        /// </summary>
        public ApplicationStatus Status { get; set; }

        /// <summary>
        /// Date when the application was submitted
        /// </summary>
        public DateTime DateApplied { get; set; }

        /// <summary>
        /// Optional contact person at the company
        /// </summary>
        public string? ContactPerson { get; set; }

        /// <summary>
        /// Optional contact email
        /// </summary>
        public string? ContactEmail { get; set; }

        /// <summary>
        /// Notes or additional information about the application
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Optional URL to the job posting
        /// </summary>
        public string? JobUrl { get; set; }

        /// <summary>
        /// Optional salary information
        /// </summary>
        public decimal? SalaryRange { get; set; }
    }
}