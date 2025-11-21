using FluentAssertions;
using Xunit;

namespace MyApi.Tests;

public class UnitTest1
{
    [Fact]
    public void Example_Test_Should_Pass()
    {
        // Arrange
        var expected = 4;

        // Act
        var actual = 2 + 2;

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(0, 32)]
    [InlineData(100, 212)]
    [InlineData(-40, -40)]
    public void WeatherForecast_TemperatureF_Should_ConvertCorrectly(int celsius, int expectedFahrenheit)
    {
        // Arrange
        var forecast = new WeatherForecast(DateOnly.FromDateTime(DateTime.Now), celsius, "Test");

        // Act
        var fahrenheit = forecast.TemperatureF;

        // Assert
        fahrenheit.Should().Be(expectedFahrenheit);
    }
}
