using Microsoft.AspNetCore.Mvc;
using MyApi.Core.Entities;
using MyApi.Core.Interfaces;

namespace MyApi.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;
    private readonly ILogger<WeatherController> _logger;

    public WeatherController(IWeatherService weatherService, ILogger<WeatherController> logger)
    {
        _weatherService = weatherService;
        _logger = logger;
    }

    /// <summary>
    /// Gets weather forecasts for the next 5 days.
    /// </summary>
    /// <param name="days">Number of days to forecast (default: 5).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of weather forecasts.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WeatherForecast>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WeatherForecast>>> GetForecast(
        [FromQuery] int days = 5,
        CancellationToken cancellationToken = default)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Getting weather forecast for {Days} days", days);
        }

        if (days < 1 || days > 30)
        {
            return BadRequest("Days must be between 1 and 30");
        }

        var forecasts = await _weatherService.GetForecastAsync(days, cancellationToken);

        return Ok(forecasts);
    }
}
