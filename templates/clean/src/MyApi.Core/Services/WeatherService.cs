using MyApi.Core.Entities;
using MyApi.Core.Interfaces;

namespace MyApi.Core.Services;

/// <summary>
/// Default implementation of weather service.
/// </summary>
public class WeatherService : IWeatherService
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    public Task<IEnumerable<WeatherForecast>> GetForecastAsync(int days, CancellationToken cancellationToken = default)
    {
#pragma warning disable CA5394 // Do not use insecure randomness - acceptable for example/demo code
        var forecasts = Enumerable.Range(1, days).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        });
#pragma warning restore CA5394

        return Task.FromResult(forecasts);
    }
}
