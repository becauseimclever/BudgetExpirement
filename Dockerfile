# Simple runtime-only Dockerfile for BudgetExperiment
# Expects the app to be pre-built and published locally to ./publish/
# Optimized for ARM64 (Raspberry Pi) with external PostgreSQL database

# Use .NET 10 preview runtime to match the application
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Install necessary dependencies 
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    ca-certificates \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Copy pre-built application (expects ./publish/ directory exists locally)
COPY ./publish/ .

# Create a non-root user for security
RUN useradd -m -u 1000 appuser && \
    chown -R appuser:appuser /app
USER appuser

# Expose port
EXPOSE 8080

# Environment variables (can be overridden at runtime)
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "BudgetExperiment.Api.dll"]
