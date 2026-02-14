using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MyApi.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults (OpenTelemetry, health checks, HTTP resilience)
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

// Add Infrastructure layer (includes Core services)
builder.Services.AddInfrastructure(builder.Configuration);

// Add API services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "MyApi",
        Version = "v1",
        Description = "A Clean Architecture API built with .NET 10"
    });
});

// Add health checks for dependencies
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("myapidb")!)
    .AddRedis(builder.Configuration.GetConnectionString("redis")!);

var app = builder.Build();

// Configure the HTTP request pipeline
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Enhanced health check endpoint with detailed response
app.MapHealthChecks("/health/detailed", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseSerilogRequestLogging();

app.Run();

// Make Program accessible for testing
public partial class Program { }
