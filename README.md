# Beacon Tower API Templates

Modern .NET 10 API project templates with Aspire and Kubernetes support.

## Templates

### 1. Minimal API Template
Lightweight API using .NET Minimal APIs pattern. Best for microservices and simple APIs.

**Features:**
- .NET 10 with C# 14
- Aspire for local orchestration
- Minimal APIs with endpoint routing
- Health checks and OpenAPI
- Kubernetes manifests included

### 2. Clean Architecture Template
Three-layer architecture (API → Core → Infrastructure). Best for medium-to-large applications with clear separation of concerns.

**Features:**
- All Minimal API features plus:
- Clean separation of concerns
- Domain-driven design friendly
- Testable business logic layer

## Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for Aspire)
- [Visual Studio 2022 17.12+](https://visualstudio.microsoft.com/) or [JetBrains Rider 2024.3+](https://www.jetbrains.com/rider/)

### Installation

#### Option 1: Using dotnet new (Recommended)

```bash
# Install the template
dotnet new install beacontower-api-template

# Create new project with Minimal API
dotnet new bt-api-minimal -n MyApi

# Create new project with Clean Architecture
dotnet new bt-api-clean -n MyApi
```

#### Option 2: Clone and Copy

```bash
# Clone this repository
git clone https://github.com/beacontower/beacontower-api-template.git

# Copy the desired template
cp -r templates/minimal my-new-api
cd my-new-api

# Replace project names
find . -type f -exec sed -i 's/MyApi/YourApiName/g' {} +
```

## Local Development with Aspire

Each template includes an Aspire AppHost project for local orchestration:

```bash
# Navigate to the AppHost project
cd src/MyApi.AppHost

# Run Aspire (starts API + dependencies)
dotnet run
```

Aspire Dashboard: http://localhost:15888

The AppHost orchestrates:
- Your API
- PostgreSQL (if needed)
- Redis (if needed)
- Other dependencies

## Project Structure

### Minimal API Template
```
MyApi/
├── src/
│   ├── MyApi/                    # Main API project
│   ├── MyApi.AppHost/            # Aspire orchestration
│   └── MyApi.ServiceDefaults/    # Shared Aspire config
├── tests/
│   ├── MyApi.Tests/              # Unit tests
│   └── MyApi.IntegrationTests/   # Integration tests
├── kubernetes/                   # K8s manifests
├── .github/workflows/            # CI/CD workflows
├── Directory.Build.props
├── Directory.Packages.props
├── .editorconfig
├── Dockerfile
└── MyApi.sln
```

## CI/CD Workflows

Each template includes three GitHub Actions workflows:

### 1. PR Validation (`pr.yml`)
Runs on pull requests to `main`:
- Build and test
- Security scanning (CodeQL, Trivy)
- Container validation
- AI code review (GPT-5.1)

### 2. Main Branch Release (`main.yml`)
Runs on push to `main`:
- Semantic versioning (automatic)
- Build and push Docker container to ACR
- Publish NuGet packages to GitHub Packages
- Create GitHub release with changelog

### 3. Branch Builds (`branches.yml`)
Runs on `feature/*`, `bugfix/*`, `hotfix/*`:
- Build and test
- Publish prerelease containers
- Publish prerelease NuGet packages

## Kubernetes Deployment

Each template includes production-ready Kubernetes manifests:

```bash
# Deploy to K8s
kubectl apply -f kubernetes/

# Deploy to K3s (integration environment)
kubectl apply -f kubernetes/k3s/
```

Manifests include:
- Deployment
- Service (ClusterIP)
- Ingress
- ConfigMap
- Secret (template)

## Configuration

### Required Secrets

Configure these in your GitHub repository:

- `AZURE_CR_TOKEN` - Azure Container Registry password
- `OPENAI_API_KEY` - OpenAI API key for PR reviews

### Required Variables

- `DOTNET_VERSION` - Default: `10.0.x`
- `CONTAINER_REGISTRY_URL` - Default: `btcontainerregistry.azurecr.io`
- `CR_TOKEN_NAME` - ACR username

## Conventional Commits

Use conventional commit format for automatic semantic versioning:

```bash
# New feature (minor version bump)
git commit -m "feat: add user authentication endpoint"

# Bug fix (patch version bump)
git commit -m "fix: resolve null reference in user service"

# Breaking change (major version bump)
git commit -m "feat!: redesign authentication API

BREAKING CHANGE: Authentication now requires OAuth2 tokens."

# No release
git commit -m "docs: update README"
git commit -m "chore: update dependencies"
```

## Documentation

See the [docs](./docs) folder for detailed guides:
- [Aspire Setup Guide](./docs/aspire-setup.md)
- [Kubernetes Deployment](./docs/kubernetes-deployment.md)
- [Architecture Decisions](./docs/architecture-decisions.md)

## Examples

Working reference implementations are in the [examples](./examples) folder.

## Support

For issues or questions:
- Create an issue in this repository
- Consult the [Beacon Tower documentation](https://docs.beacontower.io)
- Contact the Backend Team

## License

Internal use only - Beacon Tower Platform
