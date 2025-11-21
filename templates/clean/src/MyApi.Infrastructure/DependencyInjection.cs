using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyApi.Core.Interfaces;
using MyApi.Core.Services;
using MyApi.Infrastructure.Data;

namespace MyApi.Infrastructure;

/// <summary>
/// Infrastructure layer dependency injection extensions.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds infrastructure services to the service collection.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IHostApplicationBuilder builder)
    {
        // Add Database (with Aspire integration)
        builder.AddNpgsqlDbContext<ApplicationDbContext>("myapidb");

        // Add Redis (with Aspire integration)
        builder.AddRedisClient("redis");

        // Register Core services
        services.AddScoped<IWeatherService, WeatherService>();

        // Add other infrastructure services here
        // Example: services.AddScoped<IRepository<T>, Repository<T>>();

        return services;
    }
}
