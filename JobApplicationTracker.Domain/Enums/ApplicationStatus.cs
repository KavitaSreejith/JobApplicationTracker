namespace JobApplicationTracker.Domain.Enums
{
    /// <summary>
    /// Represents the current status of a job application
    /// </summary>
    public enum ApplicationStatus
    {
        /// <summary>
        /// Application has been submitted
        /// </summary>
        Applied = 0,

        /// <summary>
        /// Candidate is in the interview process
        /// </summary>
        Interview = 1,

        /// <summary>
        /// An offer has been extended to the candidate
        /// </summary>
        Offer = 2,

        /// <summary>
        /// The application has been rejected
        /// </summary>
        Rejected = 3
    }
}