using System.Text.Json;

namespace JobApplicationTracker.API.Middleware
{
    /// <summary>
    /// Model for returning error details in a standardized format
    /// </summary>
    public class ErrorDetails
    {
        /// <summary>
        /// HTTP status code
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Optional details about the error (may be null in production)
        /// </summary>
        public string? Details { get; set; }

        /// <summary>
        /// Converts the error details to a JSON string
        /// </summary>
        /// <returns>JSON representation of error details</returns>
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}