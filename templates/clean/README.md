# MyApi - Clean Architecture Template

A modern .NET 8 Clean Architecture API template with Aspire orchestration and Kubernetes support.

## Features

- **.NET 8** with **C# 12**
- **Clean Architecture** (3 layers: API → Core → Infrastructure)
- **Aspire** for local development orchestration
- **Entity Framework Core** with PostgreSQL
- **Repository Pattern** ready
- **OpenTelemetry** for observability
- **Health Checks** with dependency monitoring
- **Serilog** structured logging
- **Swagger/OpenAPI** documentation
- **XUnit** unit and integration tests
- **Docker** multi-stage build
- **Kubernetes** manifests

## Architecture

### Layer Dependencies

```
┌─────────────────────┐
│     MyApi.Api       │  ◄─── ASP.NET Core Controllers
│  (Presentation)     │       HTTP endpoints, DTOs
└──────────┬──────────┘
           │ depends on
           ↓
┌─────────────────────┐
│    MyApi.Core       │  ◄─── Business Logic
│   (Domain/App)      │       Entities, Interfaces, Services
└──────────┬──────────┘
           │ depends on
           ↓
┌─────────────────────┐
│ MyApi.Infrastructure│  ◄─── Data Access & External Services
│  (Data/External)    │       EF Core, Repositories, APIs
└─────────────────────┘
```

### Project Structure

```
MyApi/
├── src/
│   ├── MyApi.Api/                    # Presentation Layer
│   │   ├── Controllers/              # API Controllers
│   │   ├── Models/                   # DTOs, View Models
│   │   ├── Program.cs                # Application entry point
│   │   └── appsettings.json
│   ├── MyApi.Core/                   # Domain/Application Layer
│   │   ├── Entities/                 # Domain entities
│   │   ├── Interfaces/               # Service interfaces
│   │   └── Services/                 # Business logic
│   ├── MyApi.Infrastructure/         # Infrastructure Layer
│   │   ├── Data/                     # EF Core DbContext
│   │   ├── Services/                 # External service implementations
│   │   └── DependencyInjection.cs    # Infrastructure DI setup
│   ├── MyApi.AppHost/                # Aspire orchestration
│   └── MyApi.ServiceDefaults/        # Shared Aspire config
├── tests/
│   ├── MyApi.Tests/                  # Unit tests
│   └── MyApi.IntegrationTests/       # Integration tests
├── kubernetes/                       # K8s manifests
└── MyApi.sln
```

## Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- Visual Studio 2022 17.12+ or JetBrains Rider 2024.3+

### Run with Aspire

```bash
# Navigate to AppHost
cd src/MyApi.AppHost

# Run Aspire (starts API + PostgreSQL + Redis)
dotnet run
```

Open the Aspire Dashboard: http://localhost:15888

### Run API Standalone

```bash
cd src/MyApi.Api
dotnet run
```

API: http://localhost:5000
Swagger: http://localhost:5000/swagger

### Run Tests

```bash
# Unit tests
dotnet test tests/MyApi.Tests

# Integration tests
dotnet test tests/MyApi.IntegrationTests

# All tests with coverage
dotnet test /p:CollectCoverage=true
```

## API Endpoints

### Health Checks
- `GET /health` - Overall health status
- `GET /health/detailed` - Detailed health with dependencies
- `GET /alive` - Liveness probe
- `GET /ready` - Readiness probe

### API v1
- `GET /api/v1/hello` - Hello world example
- `GET /api/v1/weather?days=5` - Weather forecast (1-30 days)

### Documentation
- `/swagger` - Swagger UI
- `/swagger/v1/swagger.json` - OpenAPI spec

## Clean Architecture Principles

### 1. Core Layer (MyApi.Core)

**Purpose:** Contains business logic and domain entities.

**Rules:**
- NO external dependencies (framework-agnostic)
- Defines interfaces for data access and external services
- Contains domain entities and business rules

**Example:**
```csharp
// Entities/WeatherForecast.cs
public class WeatherForecast
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    // Business logic can be here
}

// Interfaces/IWeatherService.cs
public interface IWeatherService
{
    Task<IEnumerable<WeatherForecast>> GetForecastAsync(int days);
}
```

### 2. Infrastructure Layer (MyApi.Infrastructure)

**Purpose:** Implements Core interfaces with actual technology (EF Core, APIs, etc.).

**Rules:**
- Depends on Core layer
- Contains EF Core DbContext
- Implements repository pattern
- Configures external services

**Example:**
```csharp
// Data/ApplicationDbContext.cs
public class ApplicationDbContext : DbContext
{
    public DbSet<WeatherForecast> Forecasts { get; set; }
}

// DependencyInjection.cs
public static IServiceCollection AddInfrastructure(...)
{
    builder.AddNpgsqlDbContext<ApplicationDbContext>("myapidb");
    services.AddScoped<IWeatherService, WeatherService>();
}
```

### 3. API Layer (MyApi.Api)

**Purpose:** HTTP interface, orchestrates Core services.

**Rules:**
- Depends on Core and Infrastructure
- Thin controllers (no business logic)
- DTOs for input/output
- Dependency injection setup

**Example:**
```csharp
[ApiController]
[Route("api/v1/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;

    public async Task<ActionResult<IEnumerable<WeatherForecast>>> GetForecast(int days = 5)
    {
        var forecasts = await _weatherService.GetForecastAsync(days);
        return Ok(forecasts);
    }
}
```

## Development

### Adding a New Entity

1. Create entity in `MyApi.Core/Entities/`:
```csharp
public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
```

2. Add DbSet to `ApplicationDbContext`:
```csharp
public DbSet<Product> Products { get; set; } = null!;
```

3. Create migration:
```bash
cd src/MyApi.Infrastructure
dotnet ef migrations add AddProduct --startup-project ../MyApi.Api
dotnet ef database update --startup-project ../MyApi.Api
```

### Adding a New Service

1. Define interface in `MyApi.Core/Interfaces/`:
```csharp
public interface IProductService
{
    Task<Product?> GetByIdAsync(Guid id);
    Task<IEnumerable<Product>> GetAllAsync();
}
```

2. Implement in `MyApi.Core/Services/` or `MyApi.Infrastructure/Services/`:
```csharp
public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;
    // Implementation...
}
```

3. Register in `DependencyInjection.cs`:
```csharp
services.AddScoped<IProductService, ProductService>();
```

4. Create controller in `MyApi.Api/Controllers/`:
```csharp
[ApiController]
[Route("api/v1/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    // Implementation...
}
```

### Repository Pattern Example

```csharp
// Core/Interfaces/IRepository.cs
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}

// Infrastructure/Repositories/Repository.cs
public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    // Other implementations...
}
```

## Testing Strategy

### Unit Tests (MyApi.Tests)

Test Core layer business logic in isolation:
```csharp
public class WeatherServiceTests
{
    private readonly WeatherService _service;

    [Fact]
    public async Task Should_Return_Correct_Number_Of_Forecasts()
    {
        var result = await _service.GetForecastAsync(5);
        result.Should().HaveCount(5);
    }
}
```

### Integration Tests (MyApi.IntegrationTests)

Test entire API with WebApplicationFactory:
```csharp
public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Weather_Endpoint_Should_Return_Forecast()
    {
        var response = await _client.GetAsync("/api/v1/weather");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
```

## Database Migrations

```bash
# Add migration
cd src/MyApi.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../MyApi.Api

# Update database
dotnet ef database update --startup-project ../MyApi.Api

# Remove last migration
dotnet ef migrations remove --startup-project ../MyApi.Api

# Generate SQL script
dotnet ef migrations script --startup-project ../MyApi.Api
```

## Configuration

### Aspire (Local Development)

Aspire configures:
- PostgreSQL database with PgAdmin
- Redis cache with Redis Commander
- OpenTelemetry collector
- Service discovery

### Environment Variables

```bash
# Database (Aspire sets this automatically)
ConnectionStrings__myapidb=Host=localhost;Database=myapi;Username=postgres;Password=postgres

# Redis (Aspire sets this automatically)
ConnectionStrings__redis=localhost:6379

# OpenTelemetry
OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:4317
```

## Deployment

See the main [Kubernetes README](./kubernetes/README.md) for deployment instructions.

## Benefits of Clean Architecture

### Testability
- Core logic testable without database or HTTP
- Easy to mock dependencies
- Fast unit tests

### Maintainability
- Clear separation of concerns
- Easy to locate code
- Changes isolated to specific layers

### Flexibility
- Easy to swap EF Core for Dapper
- Easy to add new data sources
- Easy to change UI frameworks

### Independence
- Business logic doesn't depend on frameworks
- Can test business rules without infrastructure
- Can defer infrastructure decisions

## Common Patterns

### CQRS (Command Query Responsibility Segregation)

For more complex scenarios, consider adding MediatR:
```csharp
// Commands/CreateProduct/CreateProductCommand.cs
public record CreateProductCommand(string Name, decimal Price) : IRequest<Guid>;

// Commands/CreateProduct/CreateProductHandler.cs
public class CreateProductHandler : IRequestHandler<CreateProductCommand, Guid>
{
    public async Task<Guid> Handle(CreateProductCommand request, ...)
    {
        // Implementation
    }
}
```

### Domain Events

```csharp
// Core/Events/ProductCreatedEvent.cs
public record ProductCreatedEvent(Guid ProductId, string Name);

// Use in service
await _eventBus.PublishAsync(new ProductCreatedEvent(product.Id, product.Name));
```

## License

Internal use only - Beacon Tower Platform
