# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["src/WebApp/AdLawyerApi.sln", "src/WebApp/"]
COPY ["src/WebApp/WebApp.csproj", "src/WebApp/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/VkOrdApi/VkOrdApi.csproj", "src/VkOrdApi/"]

# Restore dependencies
RUN dotnet restore "src/WebApp/AdLawyerApi.sln"

# Copy everything else and build
COPY . .
RUN dotnet build "src/WebApp/WebApp.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "src/WebApp/WebApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser:appuser /app
USER appuser

# Copy published app
COPY --from=publish /app/publish .

# Expose port
EXPOSE 5000

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

# Run the app
ENTRYPOINT ["dotnet", "AdLawyerApi.dll"]

