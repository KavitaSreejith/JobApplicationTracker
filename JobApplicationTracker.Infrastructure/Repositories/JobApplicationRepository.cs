using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobApplicationTracker.Application.Interfaces;
using JobApplicationTracker.Domain.Entities;
using JobApplicationTracker.Domain.Enums;
using JobApplicationTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JobApplicationTracker.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for job application data access
    /// </summary>
    public class JobApplicationRepository : IJobApplicationRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<JobApplicationRepository> _logger;

        public JobApplicationRepository(ApplicationDbContext context, ILogger<JobApplicationRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<JobApplication>> GetAllAsync()
        {
            try
            {
                return await _context.JobApplications
                    .OrderByDescending(a => a.DateApplied)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all job applications");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<(IEnumerable<JobApplication> Applications, int TotalCount)> GetApplicationsAsync(
            int pageNumber,
            int pageSize,
            ApplicationStatus? status = null,
            string? searchTerm = null)
        {
            try
            {
                // Create base query
                var query = _context.JobApplications.AsQueryable();

                // Apply filters
                if (status.HasValue)
                {
                    query = query.Where(a => a.Status == status.Value);
                }

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    query = query.Where(a =>
                        a.CompanyName.ToLower().Contains(searchTerm) ||
                        a.Position.ToLower().Contains(searchTerm) ||
                        (a.Notes != null && a.Notes.ToLower().Contains(searchTerm)));
                }

                // Get total count before pagination
                var totalCount = await query.CountAsync();

                // Apply pagination
                var applications = await query
                    .OrderByDescending(a => a.DateApplied)
                    .ThenBy(a => a.CompanyName)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (applications, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting paginated job applications");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<JobApplication?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.JobApplications.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting job application with ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<JobApplication> CreateAsync(JobApplication jobApplication)
        {
            try
            {
                _context.JobApplications.Add(jobApplication);
                await _context.SaveChangesAsync();
                return jobApplication;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating job application");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<JobApplication> UpdateAsync(JobApplication jobApplication)
        {
            try
            {
                // Make sure UpdatedAt is set
                jobApplication.UpdatedAt = DateTime.UtcNow;

                _context.Entry(jobApplication).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return jobApplication;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Check if the record exists
                if (!await ExistsAsync(jobApplication.Id))
                {
                    _logger.LogWarning("Attempted to update non-existent job application with ID: {Id}", jobApplication.Id);
                    throw new KeyNotFoundException($"Job application with ID {jobApplication.Id} not found");
                }

                _logger.LogError(ex, "Concurrency error occurred while updating job application with ID: {Id}", jobApplication.Id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating job application with ID: {Id}", jobApplication.Id);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var jobApplication = await _context.JobApplications.FindAsync(id);
                if (jobApplication == null)
                {
                    return false;
                }

                _context.JobApplications.Remove(jobApplication);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting job application with ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                return await _context.JobApplications.AnyAsync(e => e.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if job application exists with ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<Dictionary<ApplicationStatus, int>> GetApplicationCountsByStatusAsync()
        {
            try
            {
                return await _context.JobApplications
                    .GroupBy(a => a.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Status, x => x.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting application counts by status");
                throw;
            }
        }
    }
}