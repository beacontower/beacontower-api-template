using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using MyApi.Api.Models;
using MyApi.Core.Entities;
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
        var response = await _client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Healthy");
    }

    [Fact]
    public async Task Alive_Endpoint_Should_Return_Healthy()
    {
        // Act
        var response = await _client.GetAsync("/alive");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Hello_Endpoint_Should_Return_Message()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/hello");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<HelloResponse>();
        result.Should().NotBeNull();
        result!.Message.Should().Contain("Hello");
        result.Message.Should().Contain("Clean Architecture");
    }

    [Fact]
    public async Task Weather_Endpoint_Should_Return_Forecast()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/weather");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var forecasts = await response.Content.ReadFromJsonAsync<WeatherForecast[]>();
        forecasts.Should().NotBeNull();
        forecasts.Should().HaveCount(5);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(30)]
    public async Task Weather_Endpoint_Should_Accept_Days_Parameter(int days)
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/weather?days={days}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var forecasts = await response.Content.ReadFromJsonAsync<WeatherForecast[]>();
        forecasts.Should().HaveCount(days);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(31)]
    [InlineData(-1)]
    public async Task Weather_Endpoint_Should_Reject_Invalid_Days(int days)
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/weather?days={days}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
