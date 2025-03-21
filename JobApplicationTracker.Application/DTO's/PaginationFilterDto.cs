using JobApplicationTracker.Domain.Enums;

namespace JobApplicationTracker.Application.DTOs
{
    /// <summary>
    /// DTO for pagination and filtering options
    /// </summary>
    public class PaginationFilterDto
    {
        private int _pageNumber = 1;
        private int _pageSize = 10;
        private const int MaxPageSize = 50;

        /// <summary>
        /// Page number (1-based)
        /// </summary>
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        /// <summary>
        /// Number of items per page (max 50)
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value < 1 ? 1 : value;
        }

        /// <summary>
        /// Optional filter by application status
        /// </summary>
        public ApplicationStatus? Status { get; set; }

        /// <summary>
        /// Optional search term for company name or position
        /// </summary>
        public string? SearchTerm { get; set; }
    }
}