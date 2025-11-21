# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution and project files
COPY ["Directory.Build.props", "Directory.Packages.props", "nuget.config", "./"]
COPY ["MyApi.sln", "./"]
COPY ["src/MyApi.Api/MyApi.Api.csproj", "src/MyApi.Api/"]
COPY ["src/MyApi.Core/MyApi.Core.csproj", "src/MyApi.Core/"]
COPY ["src/MyApi.Infrastructure/MyApi.Infrastructure.csproj", "src/MyApi.Infrastructure/"]
COPY ["src/MyApi.ServiceDefaults/MyApi.ServiceDefaults.csproj", "src/MyApi.ServiceDefaults/"]
COPY ["tests/MyApi.Tests/MyApi.Tests.csproj", "tests/MyApi.Tests/"]
COPY ["tests/MyApi.IntegrationTests/MyApi.IntegrationTests.csproj", "tests/MyApi.IntegrationTests/"]

# Restore dependencies
RUN dotnet restore "MyApi.sln"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/MyApi.Api"
RUN dotnet build "MyApi.Api.csproj" -c $BUILD_CONFIGURATION --no-restore -o /app/build

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MyApi.Api.csproj" -c $BUILD_CONFIGURATION --no-build -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

# Create non-root user
RUN addgroup -g 1000 appuser && \
    adduser -D -u 1000 -G appuser appuser

# Copy published files
COPY --from=publish --chown=appuser:appuser /app/publish .

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Switch to non-root user
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "MyApi.Api.dll"]
