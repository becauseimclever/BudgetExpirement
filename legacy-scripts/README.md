# Legacy Deployment Scripts

**⚠️ DEPRECATED - DO NOT USE**

These scripts are no longer used or maintained. They have been replaced by a GitHub Actions CI/CD pipeline that automatically builds and publishes Docker images to GitHub Container Registry.

## What Changed

### Old Workflow (Deprecated)
- Build Docker images locally on Windows
- Manually transfer images to Raspberry Pi via SCP
- Run deployment scripts on the Pi

### New Workflow (Current)
- GitHub Actions automatically builds multi-architecture Docker images (amd64 + arm64)
- Images are published to `ghcr.io/fortinbra/budgetexpirement`
- Raspberry Pi pulls pre-built images directly from the registry
- No local Docker builds needed

## Current Deployment Process

See **[README.Docker.md](../README.Docker.md)** for the current deployment guide.

**Quick summary:**
1. Push code to GitHub (main branch or tag)
2. GitHub Actions builds and publishes Docker image
3. On Raspberry Pi: Pull and run the image using `docker-compose.pi.yml`

## Files in This Directory

- `build-docker-windows.ps1` - Old: Built Docker images locally for ARM64
- `deploy-to-pi.ps1` - Old: Transferred images to Pi via SCP
- `deploy.ps1` - Old: Windows deployment script
- `deploy.sh` - Old: Linux deployment script
- `load-and-deploy.sh` - Old: Pi-side script to load transferred images
- `docker-compose.old.yml` - Old: Docker Compose config for local builds

## Why These Were Deprecated

1. **Inefficient**: Required Docker Desktop on Windows with buildx configured
2. **Manual Process**: Required manual SCP transfers and SSH sessions
3. **No CI/CD**: No automated testing or deployment pipeline
4. **Single Architecture**: Required separate builds for different architectures
5. **Development Confusion**: Mixed Docker and .NET local development workflows

The new CI/CD approach is simpler, more automated, and follows best practices for containerized deployments.

## If You Need These Scripts

These scripts are kept for historical reference only. If you have a specific need for local Docker builds:

1. Consider using the multi-stage Dockerfile directly: `docker build -t budgetexperiment:local .`
2. Or better: Use the standard .NET workflow for local development: `dotnet run`

For production deployments, always use the CI/CD pipeline and pull from ghcr.io.
