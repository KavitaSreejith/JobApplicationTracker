using JobApplicationTracker.Application.Interfaces;
using JobApplicationTracker.Infrastructure.Data;
using JobApplicationTracker.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JobApplicationTracker.Infrastructure
{
    /// <summary>
    /// Extension methods for dependency injection setup
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds infrastructure layer services to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The configuration</param>
        /// <returns>The updated service collection</returns>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Get database type from configuration
            var useInMemoryDatabase = configuration.GetValue<bool>("UseInMemoryDatabase");

            if (useInMemoryDatabase)
            {
                // Configure in-memory database
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("JobApplicationTrackerDb"));
            }
            else
            {
                // Configure SQLite database
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite(
                        configuration.GetConnectionString("DefaultConnection"),
                        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            }

            // Register repositories
            services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();

            return services;
        }
    }
}