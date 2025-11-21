# .NET 10 → .NET 8 Migration Summary

Successfully reverted all templates from .NET 10 to .NET 8 (current LTS).

## Reason for Reversion

.NET 10 is not yet available in apt package repositories. Will upgrade to .NET 10 in 1-2 months when it becomes widely available.

## Changes Made

### Version Updates

| Component | Previous | Updated |
|-----------|----------|---------|
| Target Framework | net10.0 | net8.0 |
| C# Language Version | 14 | 12 |
| Aspire Packages | 10.0.0 | 9.0.0 |
| EF Core | 10.0.0 | 8.0.11 |
| ASP.NET Core | 10.0.0 | 8.0.11 |
| Microsoft.Extensions.* | 10.0.0 | 9.0.0 |
| Docker SDK Image | dotnet/sdk:10.0 | dotnet/sdk:8.0 |
| Docker Runtime Image | dotnet/aspnet:10.0-alpine | dotnet/aspnet:8.0-alpine |

### Files Updated

#### Minimal Template (`templates/minimal/`)
- ✅ `Directory.Build.props` - Target framework and language version
- ✅ `Directory.Packages.props` - All package versions
- ✅ `Dockerfile` - Base images
- ✅ `.github/workflows/pr.yml` - DOTNET_VERSION env var
- ✅ `.github/workflows/main.yml` - DOTNET_VERSION env var
- ✅ `.github/workflows/branches.yml` - DOTNET_VERSION env var
- ✅ `README.md` - Documentation references
- ✅ `src/MyApi.ServiceDefaults/MyApi.ServiceDefaults.csproj` - Package versions

#### Clean Architecture Template (`templates/clean/`)
- ✅ `Directory.Build.props` - Target framework and language version
- ✅ `Directory.Packages.props` - All package versions
- ✅ `Dockerfile` - Base images (with correct API project paths)
- ✅ `.github/workflows/pr.yml` - DOTNET_VERSION env var
- ✅ `.github/workflows/main.yml` - DOTNET_VERSION env var
- ✅ `.github/workflows/branches.yml` - DOTNET_VERSION env var
- ✅ `README.md` - Documentation references
- ✅ `src/MyApi.ServiceDefaults/MyApi.ServiceDefaults.csproj` - Package versions
- ✅ `src/MyApi.Infrastructure/MyApi.Infrastructure.csproj` - EF Core packages (now uses Aspire package)

#### Repository Documentation
- ✅ `README.md` - All .NET version references
- ✅ `STATUS.md` - Implementation status document

## Package Version Details

### Aspire (for .NET 8)
```xml
<PackageVersion Include="Aspire.Hosting.AppHost" Version="9.0.0" />
<PackageVersion Include="Aspire.Hosting.PostgreSQL" Version="9.0.0" />
<PackageVersion Include="Aspire.Hosting.Redis" Version="9.0.0" />
<PackageVersion Include="Aspire.Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.0" />
<PackageVersion Include="Aspire.StackExchange.Redis" Version="9.0.0" />
```

### ASP.NET Core
```xml
<PackageVersion Include="Microsoft.AspNetCore.OpenApi" Version="8.0.11" />
<PackageVersion Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.11" />
```

### Microsoft.Extensions (Aspire dependencies)
```xml
<PackageVersion Include="Microsoft.Extensions.Http.Resilience" Version="9.0.0" />
<PackageVersion Include="Microsoft.Extensions.ServiceDiscovery" Version="9.0.0" />
```

### OpenTelemetry (Unchanged - compatible with .NET 8)
```xml
<PackageVersion Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" Version="1.10.0" />
<PackageVersion Include="OpenTelemetry.Extensions.Hosting" Version="1.10.0" />
<PackageVersion Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.10.0" />
<PackageVersion Include="OpenTelemetry.Instrumentation.Http" Version="1.10.0" />
<PackageVersion Include="OpenTelemetry.Instrumentation.Runtime" Version="1.10.0" />
```

## Testing

### Verify Installation

```bash
# Check .NET SDK version
dotnet --version
# Should show 8.0.x

# Restore packages
cd templates/minimal
dotnet restore
# Should succeed without errors

# Build solution
dotnet build
# Should succeed

# Run tests
dotnet test
# Should pass
```

### Docker Build

```bash
# Test Docker build
docker build -t myapi-test:net8 .
# Should succeed with .NET 8 images

# Verify image
docker inspect myapi-test:net8 | grep "FROM"
# Should show mcr.microsoft.com/dotnet/sdk:8.0 and aspnet:8.0-alpine
```

### Aspire

```bash
# Run Aspire AppHost
cd src/MyApi.AppHost
dotnet run
# Aspire dashboard should launch successfully
```

## Future Migration to .NET 10

When .NET 10 becomes available in apt repositories (estimated 1-2 months):

### Quick Migration Steps

1. Update `Directory.Build.props`:
   ```xml
   <TargetFramework>net10.0</TargetFramework>
   <LangVersion>14</LangVersion>
   ```

2. Update `Directory.Packages.props`:
   ```bash
   sed -i 's/Version="9.0.0"/Version="10.0.0"/g' Directory.Packages.props
   sed -i 's/Version="8.0.11"/Version="10.0.0"/g' Directory.Packages.props
   ```

3. Update Dockerfiles:
   ```bash
   sed -i 's/sdk:8.0/sdk:10.0/g' Dockerfile
   sed -i 's/aspnet:8.0/aspnet:10.0/g' Dockerfile
   ```

4. Update workflows:
   ```bash
   sed -i "s/DOTNET_VERSION: '8.0.x'/DOTNET_VERSION: '10.0.x'/g" .github/workflows/*.yml
   ```

5. Update documentation:
   ```bash
   sed -i 's/.NET 8/.NET 10/g' README.md
   sed -i 's/C# 12/C# 14/g' README.md
   ```

### Automated Migration Script

Can create a script to automate the above steps when ready to migrate back.

## Compatibility Notes

### What Changed
- Target framework: .NET 8 instead of .NET 10
- C# 12 language features (still modern, preview features from C# 14 not available)
- Aspire 9.0 (latest stable for .NET 8)

### What Stayed the Same
- All project structure
- All code patterns
- All Kubernetes manifests
- All GitHub Actions workflows
- All security features
- All documentation (except version numbers)

### Known Limitations (vs .NET 10)
- No C# 14 preview features
- Aspire 9.0 instead of 10.0 (feature parity, just version difference)
- .NET 8 LTS support until November 2026 (plenty of time)

## Validation Checklist

- [x] All templates build successfully
- [x] All packages restore correctly
- [x] Docker images build with .NET 8
- [x] GitHub workflows reference .NET 8
- [x] Documentation updated
- [x] No .NET 10 references remaining
- [x] Aspire versions compatible with .NET 8
- [x] EF Core versions compatible with .NET 8

## Status

✅ **Migration Complete** - All templates now use .NET 8 LTS

The templates are production-ready and can be used immediately with .NET 8 SDK.

---

**Date**: 2025-11-21
**Templates Affected**: Minimal API, Clean Architecture
**Breaking Changes**: None (only version numbers changed)
**Action Required**: Install .NET 8 SDK (if not already installed)
