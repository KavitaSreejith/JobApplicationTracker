using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using JobApplicationTracker.Application.DTOs;
using JobApplicationTracker.Application.Interfaces;
using JobApplicationTracker.Domain.Entities;
using JobApplicationTracker.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace JobApplicationTracker.Application.Services
{
    /// <summary>
    /// Service implementation for job application business logic
    /// </summary>
    public class JobApplicationService : IJobApplicationService
    {
        private readonly IJobApplicationRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<JobApplicationService> _logger;

        public JobApplicationService(IJobApplicationRepository repository, IMapper mapper, ILogger<JobApplicationService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task<PaginatedResponseDto<JobApplicationDto>> GetAllAsync(PaginationFilterDto filter)
        {
            try
            {
                var (applications, totalCount) = await _repository.GetApplicationsAsync(
                    filter.PageNumber,
                    filter.PageSize,
                    filter.Status,
                    filter.SearchTerm);

                var applicationDtos = _mapper.Map<IEnumerable<JobApplicationDto>>(applications);

                var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

                return new PaginatedResponseDto<JobApplicationDto>
                {
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Items = applicationDtos
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting applications with filter");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<JobApplicationDto?> GetByIdAsync(int id)
        {
            try
            {
                var jobApplication = await _repository.GetByIdAsync(id);
                return jobApplication == null ? null : _mapper.Map<JobApplicationDto>(jobApplication);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting application with ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<JobApplicationDto> CreateAsync(CreateJobApplicationDto createDto)
        {
            try
            {
                var jobApplication = _mapper.Map<JobApplication>(createDto);
                var createdApplication = await _repository.CreateAsync(jobApplication);
                return _mapper.Map<JobApplicationDto>(createdApplication);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new application");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<JobApplicationDto?> UpdateAsync(int id, UpdateJobApplicationDto updateDto)
        {
            try
            {
                var existingApplication = await _repository.GetByIdAsync(id);
                if (existingApplication == null)
                {
                    return null;
                }

                // Map updated values while preserving the ID and created date
                _mapper.Map(updateDto, existingApplication);
                existingApplication.Id = id; // Ensure ID is not changed
                existingApplication.UpdatedAt = DateTime.UtcNow;

                var updatedApplication = await _repository.UpdateAsync(existingApplication);
                return _mapper.Map<JobApplicationDto>(updatedApplication);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating application with ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<JobApplicationDto?> UpdateStatusAsync(int id, UpdateApplicationStatusDto statusDto)
        {
            try
            {
                var existingApplication = await _repository.GetByIdAsync(id);
                if (existingApplication == null)
                {
                    return null;
                }

                existingApplication.Status = statusDto.Status;
                existingApplication.UpdatedAt = DateTime.UtcNow;

                var updatedApplication = await _repository.UpdateAsync(existingApplication);
                return _mapper.Map<JobApplicationDto>(updatedApplication);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating status for application with ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                return await _repository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting application with ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<JobApplicationStatisticsDto> GetStatisticsAsync()
        {
            try
            {
                var statusCounts = await _repository.GetApplicationCountsByStatusAsync();
                var totalCount = statusCounts.Values.Sum();

                var appliedCount = statusCounts.TryGetValue(ApplicationStatus.Applied, out var applied) ? applied : 0;
                var interviewCount = statusCounts.TryGetValue(ApplicationStatus.Interview, out var interview) ? interview : 0;
                var offerCount = statusCounts.TryGetValue(ApplicationStatus.Offer, out var offer) ? offer : 0;
                var rejectedCount = statusCounts.TryGetValue(ApplicationStatus.Rejected, out var rejected) ? rejected : 0;

                var successRate = totalCount > 0
                    ? Math.Round((decimal)offerCount / totalCount * 100, 2)
                    : 0;

                var interviewRate = appliedCount > 0
                    ? Math.Round((decimal)interviewCount / (interviewCount + appliedCount) * 100, 2)
                    : 0;

                return new JobApplicationStatisticsDto
                {
                    TotalCount = totalCount,
                    AppliedCount = appliedCount,
                    InterviewCount = interviewCount,
                    OfferCount = offerCount,
                    RejectedCount = rejectedCount,
                    SuccessRate = successRate,
                    InterviewRate = interviewRate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting application statistics");
                throw;
            }
        }
    }
}