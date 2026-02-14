using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyApi.Core.Interfaces;
using MyApi.Core.Services;
using MyApi.Infrastructure.Data;

namespace MyApi.Infrastructure;

/// <summary>
/// Infrastructure layer dependency injection extensions.
/// </summary>
public static class InfrastructureServiceExtensions
{
    /// <summary>
    /// Adds infrastructure services to the service collection.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("myapidb")));

        // Add Redis distributed cache
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("redis");
        });

        // Register Core services
        services.AddScoped<IWeatherService, WeatherService>();

        return services;
    }
}
