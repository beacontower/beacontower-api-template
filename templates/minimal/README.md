# MyApi - Minimal API Template

A modern .NET 8 Minimal API template with Aspire orchestration and Kubernetes support.

## Features

- **.NET 8** with **C# 12**
- **Aspire** for local development orchestration
- **Minimal APIs** for lightweight endpoints
- **OpenTelemetry** for observability (metrics, tracing, logging)
- **Health Checks** (liveness, readiness)
- **Serilog** structured logging
- **Swagger/OpenAPI** documentation
- **XUnit** unit and integration tests
- **Docker** multi-stage build
- **Kubernetes** manifests
- **GitHub Actions** CI/CD workflows
- **Semantic Versioning** with conventional commits

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

The API will be available at: http://localhost:5000 (or the port shown in Aspire Dashboard)

### Run API Standalone

```bash
cd src/MyApi
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
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Build Docker Container

```bash
docker build -t myapi:latest .
docker run -p 8080:8080 myapi:latest
```

## Project Structure

```
MyApi/
├── src/
│   ├── MyApi/                    # Main API project
│   │   ├── Program.cs            # Minimal API endpoints
│   │   └── appsettings.json
│   ├── MyApi.AppHost/            # Aspire orchestration
│   │   └── Program.cs            # Aspire app definition
│   └── MyApi.ServiceDefaults/    # Shared Aspire configuration
│       └── Extensions.cs         # OpenTelemetry, health checks
├── tests/
│   ├── MyApi.Tests/              # Unit tests
│   └── MyApi.IntegrationTests/   # Integration tests with WebApplicationFactory
├── kubernetes/                   # K8s manifests
├── Dockerfile                    # Multi-stage Docker build
├── MyApi.sln
└── README.md
```

## API Endpoints

### Health Checks
- `GET /health` - Overall health status
- `GET /alive` - Liveness probe (for K8s)
- `GET /ready` - Readiness probe (for K8s)

### API v1
- `GET /api/v1/hello` - Hello world example
- `GET /api/v1/weather` - Weather forecast example

### Documentation
- `/swagger` - Swagger UI (development only)
- `/swagger/v1/swagger.json` - OpenAPI specification

## Configuration

### Aspire Dependencies

The AppHost configures:
- **PostgreSQL** with PgAdmin UI
- **Redis** with Redis Commander UI
- **OpenTelemetry** collector

Access UIs from Aspire Dashboard.

### Environment Variables

```bash
# Aspire OpenTelemetry
OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:4317

# Logging
Serilog__MinimumLevel__Default=Information
```

## Development

### Adding New Endpoints

Edit `src/MyApi/Program.cs`:

```csharp
api.MapGet("/api/v1/my-endpoint", () =>
{
    return Results.Ok(new { Message = "Hello!" });
})
    .WithName("MyEndpoint")
    .WithOpenApi()
    .WithTags("MyFeature");
```

### Adding Dependencies to Aspire

Edit `src/MyApi.AppHost/Program.cs`:

```csharp
// Add a new service
var mongodb = builder.AddMongoDB("mongodb");
var db = mongodb.AddDatabase("mydb");

// Reference it in the API
var api = builder.AddProject<Projects.MyApi>("myapi")
    .WithReference(db);
```

## Deployment

### Kubernetes

```bash
# Deploy to K8s
kubectl apply -f kubernetes/

# Deploy to K3s (integration)
kubectl apply -f kubernetes/k3s/
```

### CI/CD

GitHub Actions workflows:
- **PR Validation** (`.github/workflows/pr.yml`) - Build, test, security scan, AI review
- **Main Release** (`.github/workflows/main.yml`) - Semantic version, build container, push to registry
- **Branch Builds** (`.github/workflows/branches.yml`) - Prerelease builds

## Testing

### Unit Tests

Located in `tests/MyApi.Tests/`. Uses XUnit, FluentAssertions, and Moq.

### Integration Tests

Located in `tests/MyApi.IntegrationTests/`. Uses WebApplicationFactory and Testcontainers.

## Observability

### Metrics
- ASP.NET Core metrics (requests, duration)
- HTTP client metrics
- Runtime metrics (GC, threads, memory)

### Tracing
- Distributed tracing with OpenTelemetry
- HTTP request/response traces

### Logging
- Structured logging with Serilog
- JSON output in production
- Human-readable console in development

View telemetry in Aspire Dashboard when running locally.

## Security

- **Non-root Docker container** (runs as `appuser`)
- **Health checks** for container orchestration
- **HTTPS** redirect in production
- **Security scanning** in CI/CD (CodeQL, Trivy)

## License

Internal use only - Beacon Tower Platform
