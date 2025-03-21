using System.Reflection;
using JobApplicationTracker.Application.Interfaces;
using JobApplicationTracker.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace JobApplicationTracker.Application
{
    /// <summary>
    /// Extension methods for dependency injection setup
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds application layer services to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The updated service collection</returns>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Register application services
            services.AddScoped<IJobApplicationService, JobApplicationService>();

            return services;
        }
    }
}