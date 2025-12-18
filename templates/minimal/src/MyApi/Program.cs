using MyApi;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components
builder.AddServiceDefaults();

// Configure Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console());

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "MyApi",
        Version = "v1",
        Description = "A minimal API built with .NET 10 and Aspire"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Minimal API endpoints
var api = app.MapGroup("/api/v1");

api.MapGet("/hello", () => Results.Ok(new { Message = "Hello from MyApi!", Timestamp = DateTime.UtcNow }))
    .WithName("Hello")
    .WithOpenApi()
    .WithTags("General");

api.MapGet("/weather", () =>
{
#pragma warning disable CA5394 // Do not use insecure randomness - acceptable for example/demo code
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            WeatherForecast.Summaries[Random.Shared.Next(WeatherForecast.Summaries.Length)]
        ))
        .ToArray();
#pragma warning restore CA5394
    return Results.Ok(forecast);
})
    .WithName("GetWeatherForecast")
    .WithOpenApi()
    .WithTags("Weather");

app.UseSerilogRequestLogging();

app.Run();

namespace MyApi
{
    public sealed record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        internal static readonly string[] Summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

        public int TemperatureF => (int)Math.Round(TemperatureC * 1.8 + 32);
    }
}
