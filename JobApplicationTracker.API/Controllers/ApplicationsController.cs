using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using JobApplicationTracker.Application.DTOs;
using JobApplicationTracker.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JobApplicationTracker.API.Controllers
{
    /// <summary>
    /// API controller for job applications
    /// </summary>
    public class ApplicationsController : BaseApiController
    {
        private readonly IJobApplicationService _jobApplicationService;
        private readonly ILogger<ApplicationsController> _logger;

        public ApplicationsController(IJobApplicationService jobApplicationService, ILogger<ApplicationsController> logger)
        {
            _jobApplicationService = jobApplicationService ?? throw new ArgumentNullException(nameof(jobApplicationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets all job applications with optional filtering and pagination
        /// </summary>
        /// <param name="status">Optional status filter</param>
        /// <param name="searchTerm">Optional search term for company or position</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 50)</param>
        /// <returns>A list of job applications</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginatedResponseDto<JobApplicationDto>>> GetApplications(
            [FromQuery] int? status,
            [FromQuery] string? searchTerm,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var filter = new PaginationFilterDto
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Status = status.HasValue ? (Domain.Enums.ApplicationStatus)status.Value : null,
                    SearchTerm = searchTerm
                };

                var result = await _jobApplicationService.GetAllAsync(filter);

                // Add pagination info to response headers
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(new
                {
                    result.TotalCount,
                    result.TotalPages,
                    result.PageNumber,
                    result.PageSize,
                    result.HasPreviousPage,
                    result.HasNextPage
                }));

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving applications");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Gets a specific job application by ID
        /// </summary>
        /// <param name="id">The ID of the job application</param>
        /// <returns>The job application if found</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JobApplicationDto>> GetApplication(int id)
        {
            try
            {
                var application = await _jobApplicationService.GetByIdAsync(id);

                if (application == null)
                {
                    _logger.LogInformation("Job application with ID {Id} not found", id);
                    return NotFound();
                }

                return Ok(application);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving job application with ID {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Creates a new job application
        /// </summary>
        /// <param name="createDto">The job application data</param>
        /// <returns>The created job application</returns>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JobApplicationDto>> CreateApplication(CreateJobApplicationDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdApplication = await _jobApplicationService.CreateAsync(createDto);

                return CreatedAtAction(
                    nameof(GetApplication),
                    new { id = createdApplication.Id },
                    createdApplication);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating job application");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Updates an existing job application
        /// </summary>
        /// <param name="id">The ID of the job application to update</param>
        /// <param name="updateDto">The updated job application data</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateApplication(int id, UpdateJobApplicationDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedApplication = await _jobApplicationService.UpdateAsync(id, updateDto);

                if (updatedApplication == null)
                {
                    _logger.LogInformation("Job application with ID {Id} not found", id);
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating job application with ID {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Updates only the status of a job application
        /// </summary>
        /// <param name="id">The ID of the job application</param>
        /// <param name="statusDto">The status update data</param>
        /// <returns>No content if successful</returns>
        [HttpPatch("{id}/status")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateApplicationStatus(int id, UpdateApplicationStatusDto statusDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedApplication = await _jobApplicationService.UpdateStatusAsync(id, statusDto);

                if (updatedApplication == null)
                {
                    _logger.LogInformation("Job application with ID {Id} not found", id);
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating status for job application with ID {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Deletes a job application
        /// </summary>
        /// <param name="id">The ID of the job application to delete</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteApplication(int id)
        {
            try
            {
                var result = await _jobApplicationService.DeleteAsync(id);

                if (!result)
                {
                    _logger.LogInformation("Job application with ID {Id} not found", id);
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting job application with ID {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Gets job application statistics
        /// </summary>
        /// <returns>Statistics about job applications</returns>
        [HttpGet("statistics")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JobApplicationStatisticsDto>> GetStatistics()
        {
            try
            {
                var statistics = await _jobApplicationService.GetStatisticsAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving job application statistics");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}