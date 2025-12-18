using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace MyApi.IntegrationTests;

public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Health_Endpoint_Should_Return_Healthy()
    {
        // Act
        var response = await _client.GetAsync(new Uri("/health", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Healthy");
    }

    [Fact]
    public async Task Alive_Endpoint_Should_Return_Healthy()
    {
        // Act
        var response = await _client.GetAsync(new Uri("/alive", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Hello_Endpoint_Should_Return_Message()
    {
        // Act
        var response = await _client.GetAsync(new Uri("/api/v1/hello", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<HelloResponse>();
        result.Should().NotBeNull();
        result!.Message.Should().Contain("Hello");
    }

    [Fact]
    public async Task Weather_Endpoint_Should_Return_Forecast()
    {
        // Act
        var response = await _client.GetAsync(new Uri("/api/v1/weather", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var forecasts = await response.Content.ReadFromJsonAsync<WeatherForecast[]>();
        forecasts.Should().NotBeNull();
        forecasts.Should().HaveCount(5);
    }

    private record HelloResponse(string Message, DateTime Timestamp);
}
