using MyApi.Core.Entities;

namespace MyApi.Core.Interfaces;

/// <summary>
/// Service for retrieving weather forecasts.
/// </summary>
public interface IWeatherService
{
    /// <summary>
    /// Gets weather forecasts for the specified number of days.
    /// </summary>
    /// <param name="days">Number of days to forecast.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of weather forecasts.</returns>
    Task<IEnumerable<WeatherForecast>> GetForecastAsync(int days, CancellationToken cancellationToken = default);
}
