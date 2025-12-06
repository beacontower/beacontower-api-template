# MyApi

A .NET 10 Minimal API with Aspire orchestration for Beacon Tower.

## Setup After Creating From Template

### 1. Initialize Git & Push to GitHub

```bash
git init && git branch -m main
dotnet build && dotnet test
git add . && git commit -m "chore: initial project setup"
git tag v0.0.0  # Ensures first release is 0.1.0
gh repo create beacontower/your-repo-name --public --source=. --push
git push origin v0.0.0
```

### 2. Configure Repository Settings

Go to **Settings → Actions → General**:
- Workflow permissions: **Read and write permissions**
- Check: **Allow GitHub Actions to create and approve pull requests**

### 3. Add Repository Secrets/Variables

Required for container publishing:
- `AZURE_CR_TOKEN` (secret) - Azure Container Registry token
- `CONTAINER_REGISTRY_URL` (variable) - e.g., `btcontainerregistry.azurecr.io`
- `CR_TOKEN_NAME` (variable) - Token username

### 4. First Release

Commit with `feat:` to trigger the first release (0.1.0):
```bash
git add . && git commit -m "feat: initial API implementation"
git push
```

## Development

### Run with Aspire

```bash
cd src/MyApi.AppHost
dotnet run
```

Open the Aspire Dashboard: http://localhost:15888

### Run API Standalone

```bash
cd src/MyApi
dotnet run
```

- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger

### Run Tests

```bash
dotnet test
```

### Build Docker Container

```bash
docker build -t myapi:latest .
docker run -p 8080:8080 myapi:latest
```

## CI/CD

- **PR workflow**: Build, test, security scan
- **Main workflow**: Semantic release + build/push container

Use conventional commits for automatic versioning:
- `feat:` → minor version bump (0.1.0 → 0.2.0)
- `fix:` → patch version bump (0.1.0 → 0.1.1)
- `feat!:` or `BREAKING CHANGE:` → major version bump (0.1.0 → 1.0.0)

## Deployment

```bash
kubectl apply -f kubernetes/
```
