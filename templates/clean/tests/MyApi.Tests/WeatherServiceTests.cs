using FluentAssertions;
using MyApi.Core.Services;
using Xunit;

namespace MyApi.Tests;

public class WeatherServiceTests
{
    private readonly WeatherService _weatherService;

    public WeatherServiceTests()
    {
        _weatherService = new WeatherService();
    }

    [Fact]
    public async Task GetForecastAsync_Should_Return_Correct_Number_Of_Forecasts()
    {
        // Arrange
        const int days = 5;

        // Act
        var result = await _weatherService.GetForecastAsync(days);

        // Assert
        result.Should().HaveCount(days);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task GetForecastAsync_Should_Return_Future_Dates(int days)
    {
        // Act
        var result = await _weatherService.GetForecastAsync(days);

        // Assert
        result.Should().AllSatisfy(forecast =>
        {
            forecast.Date.Should().BeAfter(DateOnly.FromDateTime(DateTime.Now));
        });
    }

    [Fact]
    public async Task GetForecastAsync_Should_Have_Valid_Temperature_Range()
    {
        // Act
        var result = await _weatherService.GetForecastAsync(5);

        // Assert
        result.Should().AllSatisfy(forecast =>
        {
            forecast.TemperatureC.Should().BeInRange(-20, 55);
        });
    }

    [Fact]
    public async Task GetForecastAsync_Should_Calculate_Fahrenheit_Correctly()
    {
        // Act
        var result = await _weatherService.GetForecastAsync(1);
        var forecast = result.First();

        // Assert
        var expectedF = 32 + (int)(forecast.TemperatureC / 0.5556);
        forecast.TemperatureF.Should().Be(expectedF);
    }
}
