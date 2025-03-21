namespace JobApplicationTracker.Application.DTOs
{
    /// <summary>
    /// DTO for job application statistics
    /// </summary>
    public class JobApplicationStatisticsDto
    {
        /// <summary>
        /// Total number of job applications
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Number of applications with 'Applied' status
        /// </summary>
        public int AppliedCount { get; set; }

        /// <summary>
        /// Number of applications with 'Interview' status
        /// </summary>
        public int InterviewCount { get; set; }

        /// <summary>
        /// Number of applications with 'Offer' status
        /// </summary>
        public int OfferCount { get; set; }

        /// <summary>
        /// Number of applications with 'Rejected' status
        /// </summary>
        public int RejectedCount { get; set; }

        /// <summary>
        /// Percentage of applications resulting in offers (Offers / Total)
        /// </summary>
        public decimal SuccessRate { get; set; }

        /// <summary>
        /// Percentage of applications resulting in interviews (Interviews / Applied)
        /// </summary>
        public decimal InterviewRate { get; set; }
    }
}