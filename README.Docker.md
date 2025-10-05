# Docker Deployment Guide for BudgetExperiment# Docker Deployment Guide for BudgetExperiment



This guide covers deploying the BudgetExperiment application using Docker with automated CI/CD.This guide covers deploying the BudgetExperiment application to a Raspberry Pi using Docker.



## Table of Contents## Table of Contents



1. [Overview](#overview)1. [Prerequisites](#prerequisites)

2. [CI/CD Pipeline](#cicd-pipeline)2. [Deployment Methods](#deployment-methods)

3. [Deploying to Raspberry Pi](#deploying-to-raspberry-pi)   - [Method 1: Build on Windows, Deploy to Pi (Recommended)](#method-1-build-on-windows-deploy-to-pi-recommended)

4. [Local Development](#local-development)   - [Method 2: Build Directly on Pi](#method-2-build-directly-on-pi)

5. [Configuration](#configuration)3. [Configuration](#configuration)

6. [Management Commands](#management-commands)4. [Management Commands](#management-commands)

7. [Troubleshooting](#troubleshooting)5. [Troubleshooting](#troubleshooting)



## Overview## Prerequisites



BudgetExperiment uses a GitHub Actions workflow to automatically build and publish multi-architecture Docker images (amd64 and arm64) to GitHub Container Registry (ghcr.io). This means:### On Raspberry Pi

- Raspberry Pi (3B+ or newer recommended) running a 64-bit OS

- **No local Docker builds required** - Images are built in CI- Docker and Docker Compose installed on the Raspberry Pi

- **Multi-architecture support** - Works on x86_64 servers and ARM Raspberry Pi- PostgreSQL database server (running separately)

- **Automated deployments** - Push to main or create a tag to trigger builds- Network access from Raspberry Pi to PostgreSQL server

- **Local development uses standard .NET workflow** - `dotnet run` (no Docker needed locally)- SSH access enabled



## CI/CD Pipeline### On Windows Development Machine (for Method 1)

- Docker Desktop with buildx support

### Automatic Image Builds- SSH client (built into Windows 10/11 or use PuTTY)

- SCP client (built into Windows 10/11)

The GitHub Actions workflow (`.github/workflows/docker-build-publish.yml`) automatically builds Docker images when:- Network access to Raspberry Pi



- **Push to `main` branch** - Creates `latest` tag## Deployment Methods

- **Create a version tag** (e.g., `v1.0.0`) - Creates versioned tags

- **Pull requests** - Builds but doesn't push (validation only)### Method 1: Build on Windows, Deploy to Pi (Recommended)



Images are published to: `ghcr.io/fortinbra/budgetexpirement:latest`This method builds the Docker image on your Windows machine and transfers it to the Raspberry Pi. This is faster and avoids taxing the Pi's limited resources.



### Image Tags#### Step 1: Build Docker Image on Windows



The workflow creates multiple tags:```powershell

- `latest` - Latest from main branch# From the project root directory

- `main` - Same as latest.\build-docker-windows.ps1

- `v1.0.0` - Semantic version tags```

- `v1.0` - Major.minor version

- `v1` - Major version onlyThis script will:

- `main-<sha>` - Specific commit SHA- Run all unit tests

- Publish the application for linux-arm64

## Deploying to Raspberry Pi- Build a Docker image using buildx for ARM64 architecture



### Prerequisites#### Step 2: Deploy to Raspberry Pi (All-in-One)



1. **Raspberry Pi** (3B+ or newer with 64-bit OS recommended)```powershell

2. **Docker and Docker Compose installed** on the Pi# Replace with your Raspberry Pi's hostname/IP and username

3. **PostgreSQL database** (running separately, not in container).\deploy-to-pi.ps1 -PiHost raspberry-pi.local -PiUser pi

4. **GitHub Personal Access Token** with `read:packages` scope```



### Step 1: Install Docker on Raspberry PiThis script will:

- Build the Docker image on Windows

If Docker is not already installed:- Save the image to a tar file

- Transfer the image and configuration files to the Pi via SCP

```bash- Load the image on the Pi

# Update system- Deploy using docker-compose

sudo apt-get update && sudo apt-get upgrade -y

**First-time setup:**

# Install DockerBefore running the deployment script, ensure you have:

curl -fsSL https://get.docker.com -o get-docker.sh

sudo sh get-docker.sh1. **SSH Access**: Set up SSH key authentication (recommended) or have password ready:

   ```powershell

# Add current user to docker group   # Generate SSH key (if you don't have one)

sudo usermod -aG docker $USER   ssh-keygen -t rsa -b 4096

   

# Install Docker Compose   # Copy key to Raspberry Pi

sudo apt-get install -y docker-compose-plugin   type $env:USERPROFILE\.ssh\id_rsa.pub | ssh pi@raspberry-pi.local "mkdir -p ~/.ssh && cat >> ~/.ssh/authorized_keys"

   ```

# Verify installation

docker --version2. **Database Configuration on Pi**: Create a `.env` file on your Raspberry Pi:

docker compose version   ```bash

```   # SSH to your Pi

   ssh pi@raspberry-pi.local

Log out and back in for group changes to take effect.   

   # Create deployment directory

### Step 2: Authenticate with GitHub Container Registry   mkdir -p ~/BudgetExperiment

   cd ~/BudgetExperiment

```bash   

# Create a Personal Access Token at: https://github.com/settings/tokens   # Create .env file

# Grant it 'read:packages' permission   nano .env

   ```

# Log in to ghcr.io   

docker login ghcr.io -u YOUR_GITHUB_USERNAME -p YOUR_GITHUB_TOKEN   Add your database connection string:

```   ```env

   DB_CONNECTION_STRING=Host=192.168.1.100;Port=5432;Database=budgetexperiment;Username=budgetuser;Password=YourSecurePassword123!

### Step 3: Create Deployment Directory   ```



```bash#### Manual Step-by-Step Deployment

mkdir -p ~/BudgetExperiment

cd ~/BudgetExperimentIf you prefer to run each step manually:

```

```powershell

### Step 4: Download Deployment Files# Step 1: Build image

.\build-docker-windows.ps1

Download `docker-compose.pi.yml` from the repository:

# Step 2: Save image to tar file

```bashdocker save -o budgetexperiment-image.tar budgetexperiment:latest

# Option 1: Using wget

wget https://raw.githubusercontent.com/Fortinbra/BudgetExpirement/main/docker-compose.pi.yml# Step 3: Transfer to Pi

scp budgetexperiment-image.tar pi@raspberry-pi.local:/home/pi/BudgetExperiment/

# Option 2: Using curlscp docker-compose.yml pi@raspberry-pi.local:/home/pi/BudgetExperiment/

curl -O https://raw.githubusercontent.com/Fortinbra/BudgetExpirement/main/docker-compose.pi.ymlscp load-and-deploy.sh pi@raspberry-pi.local:/home/pi/BudgetExperiment/



# Option 3: Copy from your development machine via SCP# Step 4: SSH to Pi and deploy

# (Run this from your Windows machine)ssh pi@raspberry-pi.local

scp c:\ws\BudgetExpirement\docker-compose.pi.yml pi@raspberry-pi.local:~/BudgetExperiment/cd ~/BudgetExperiment

```chmod +x load-and-deploy.sh

./load-and-deploy.sh

### Step 5: Create Environment File```



Create a `.env` file with your database connection string:### Method 2: Build Directly on Pi



```bashThis method builds everything directly on the Raspberry Pi. It's simpler but slower and more resource-intensive.

nano .env

```## Quick Start



Add your database connection string:### Install Docker on Raspberry Pi



```envIf Docker is not already installed on your Raspberry Pi:

DB_CONNECTION_STRING=Host=192.168.1.100;Port=5432;Database=budgetexperiment;Username=budgetuser;Password=YourSecurePassword123!

``````bash

# Update system

Save and secure the file:sudo apt-get update && sudo apt-get upgrade -y



```bash# Install Docker

chmod 600 .envcurl -fsSL https://get.docker.com -o get-docker.sh

```sudo sh get-docker.sh



**Important**: Never commit `.env` to version control!# Add current user to docker group (avoid sudo)

sudo usermod -aG docker $USER

### Step 6: Deploy the Application

# Install Docker Compose

```bashsudo apt-get install -y docker-compose

# Pull the latest image and start

docker compose -f docker-compose.pi.yml up -d# Verify installation

docker --version

# View logsdocker-compose --version

docker compose -f docker-compose.pi.yml logs -f```



# Check statusLog out and back in for group changes to take effect.

docker compose -f docker-compose.pi.yml ps

```### Clone or Transfer Project Files



### Step 7: Verify DeploymentCreate a `.env` file with your PostgreSQL connection string:



```bash```bash

# Test health endpoint# Copy the example file

curl http://localhost:5099/healthcp .env.example .env



# Should return: {"status":"Healthy"}# Edit with your actual connection details

```nano .env

```

## Application URLs

Example `.env` content:

Once deployed, access the application at:

```env

- **Web UI**: `http://your-raspberry-pi-ip:5099`DB_CONNECTION_STRING=Host=192.168.1.100;Port=5432;Database=budgetexperiment;Username=budgetuser;Password=YourSecurePassword123!

- **API Documentation (Scalar)**: `http://your-raspberry-pi-ip:5099/scalar````

- **OpenAPI Spec**: `http://your-raspberry-pi-ip:5099/openapi/v1.json`

- **Health Check**: `http://your-raspberry-pi-ip:5099/health`**Important**: Never commit `.env` to version control! It's already in `.gitignore`.



## Local Development### Deploy the Application



**IMPORTANT**: For local development, do NOT use Docker. Use the standard .NET development workflow.#### Using the deployment script (if files transferred from Windows)



### Running Locally```bash

# Make the script executable

```powershellchmod +x deploy.sh

# From the repository root

dotnet run --project c:\ws\BudgetExpirement\src\BudgetExperiment.Api\BudgetExperiment.Api.csproj# Run the deployment

```./deploy.sh

#### Building from source on the Pi

The API automatically serves the Blazor WebAssembly client. Only run the API project - never run the Client project standalone.

If you cloned the repository and want to build on the Pi:

Access locally at: `http://localhost:5099`

```bash

### Local Database Configuration# Make the script executable

chmod +x deploy.sh

Store your connection string in user secrets (never in code):

# Run the deployment (builds and deploys)

```powershell./deploy.sh

# Set connection string```

dotnet user-secrets set "ConnectionStrings:AppDb" "Host=localhost;Port=5432;Database=budgetexperiment;Username=postgres;Password=yourpassword" --project c:\ws\BudgetExpirement\src\BudgetExperiment.Api\BudgetExperiment.Api.csproj

```This will run tests, build the Docker image on the Pi, and deploy with docker-compose.



## Configuration### Verify Deployment



### Database Connection String Format```bash

# Check container status

The connection string follows Npgsql format:docker-compose ps



```# View logs

Host=hostname;Port=5432;Database=dbname;Username=user;Password=pass;[options]docker-compose logs -f

```

# Test health endpoint

Common options:curl http://localhost:5099/health

- `SSL Mode=Require` - Force SSL connection

- `SSL Mode=Prefer` - Prefer SSL if available# Access the application

- `Pooling=true` - Enable connection pooling (default)# Open browser: http://your-raspberry-pi-ip:5099

- `Minimum Pool Size=1` - Minimum connections in pool```

- `Maximum Pool Size=20` - Maximum connections in pool

- `Command Timeout=30` - Command timeout in seconds## Application URLs



### Port ConfigurationOnce deployed, the application is accessible at:



Default port is 5099. To change, edit `docker-compose.pi.yml`:- **Web UI**: `http://your-raspberry-pi-ip:5099`

- **API Documentation (Scalar)**: `http://your-raspberry-pi-ip:5099/scalar`

```yaml- **OpenAPI Spec**: `http://your-raspberry-pi-ip:5099/openapi/v1.json`

ports:- **Health Check**: `http://your-raspberry-pi-ip:5099/health`

  - "YOUR_PORT:8080"

```## Management Commands



### Resource Limits### View Logs

```bash

Default resource limits in `docker-compose.pi.yml`:# Follow logs in real-time

docker-compose logs -f

- **CPU Limit**: 2 cores

- **Memory Limit**: 1GB# View last 50 lines

- **Memory Reservation**: 256MBdocker-compose logs --tail=50



For Raspberry Pi 3B (1GB RAM), reduce these limits:# View specific service logs

docker-compose logs -f budgetexperiment

```yaml```

deploy:

  resources:### Stop/Start/Restart

    limits:```bash

      cpus: '2'# Stop the application

      memory: 512Mdocker-compose stop

    reservations:

      cpus: '0.25'# Start the application

      memory: 128Mdocker-compose start

```

# Restart the application

## Management Commandsdocker-compose restart



### Updating to Latest Image# Stop and remove containers (but keep data)

docker-compose down

```bash

cd ~/BudgetExperiment# Stop, remove containers, and remove images

docker-compose down --rmi all

# Pull latest image```

docker compose -f docker-compose.pi.yml pull

### Update Deployment

# Restart with new image```bash

docker compose -f docker-compose.pi.yml up -d# After code changes, rebuild and redeploy

docker-compose down

# View logsdocker build -t budgetexperiment:latest .

docker compose -f docker-compose.pi.yml logs -fdocker-compose up -d

``````



### Viewing LogsOr simply run:

```bash

```bash./deploy.sh

# Follow logs in real-time```

docker compose -f docker-compose.pi.yml logs -f

## Configuration

# View last 50 lines

docker compose -f docker-compose.pi.yml logs --tail=50### Database Connection String Format



# View specific service logsThe connection string follows Npgsql format:

docker compose -f docker-compose.pi.yml logs -f budgetexperiment

``````

Host=hostname;Port=5432;Database=dbname;Username=user;Password=pass;[options]

### Stop/Start/Restart```



```bashCommon options:

# Stop the application- `SSL Mode=Require` - Force SSL connection

docker compose -f docker-compose.pi.yml stop- `SSL Mode=Prefer` - Prefer SSL if available

- `Pooling=true` - Enable connection pooling (default)

# Start the application- `Minimum Pool Size=1` - Minimum connections in pool

docker compose -f docker-compose.pi.yml start- `Maximum Pool Size=20` - Maximum connections in pool

- `Command Timeout=30` - Command timeout in seconds

# Restart the application

docker compose -f docker-compose.pi.yml restart### Port Configuration



# Stop and remove containersBy default, the application runs on port 5099. To change:

docker compose -f docker-compose.pi.yml down

1. Edit `docker-compose.yml`:

# Stop, remove containers, and remove images   ```yaml

docker compose -f docker-compose.pi.yml down --rmi all   ports:

```     - "YOUR_PORT:8080"

   ```

## Troubleshooting

2. Or set in `.env`:

### Container fails to start   ```env

   BUDGET_APP_PORT=8080

```bash   ```

# Check container logs

docker compose -f docker-compose.pi.yml logs budgetexperiment### Resource Limits



# Common issues:Default resource limits (adjust in `docker-compose.yml` based on your Pi model):

# 1. Database connection - verify connection string in .env

# 2. Port conflicts - ensure port 5099 is not in use- **CPU Limit**: 2 cores

# 3. Resource limits - adjust if Raspberry Pi is constrained- **Memory Limit**: 1GB

# 4. Image pull errors - verify ghcr.io authentication- **Memory Reservation**: 256MB

```

For Raspberry Pi 3B (1GB RAM):

### Authentication errors pulling image```yaml

deploy:

```bash  resources:

# Re-authenticate with GitHub Container Registry    limits:

docker login ghcr.io -u YOUR_GITHUB_USERNAME -p YOUR_GITHUB_TOKEN      cpus: '2'

      memory: 512M

# Verify you can pull the image    reservations:

docker pull ghcr.io/fortinbra/budgetexpirement:latest      cpus: '0.25'

```      memory: 128M

```

### Database connection errors

For Raspberry Pi 4+ (4GB+ RAM): Current defaults are fine.

```bash

# Test database connectivity from Raspberry Pi## Troubleshooting

# Install PostgreSQL client if needed

sudo apt-get install -y postgresql-client### Container fails to start



# Test connection (replace with your details)```bash

psql -h your-db-server -U your-username -d budgetexperiment# Check container logs

docker-compose logs budgetexperiment

# If connection fails, check:

# 1. PostgreSQL pg_hba.conf allows connections from Pi's IP# Check for common issues:

# 2. PostgreSQL is listening on the correct interface (postgresql.conf)# 1. Database connection - verify connection string in .env

# 3. Firewall rules allow port 5432# 2. Port conflicts - ensure port 5099 is not in use

# 4. Network connectivity between Pi and DB server# 3. Resource limits - adjust if Raspberry Pi is constrained

``````



### Performance issues on Raspberry Pi### Database connection errors



```bash```bash

# Monitor resource usage# Test database connectivity from Raspberry Pi

docker stats budgetexperiment# Install PostgreSQL client if needed

sudo apt-get install -y postgresql-client

# If high CPU/memory:

# 1. Reduce resource limits in docker-compose.pi.yml# Test connection (replace with your details)

# 2. Consider upgrading to Pi 4 with more RAMpsql -h your-db-server -U your-username -d budgetexperiment

# 3. Verify database queries are optimized

```# If connection fails, check:

# 1. PostgreSQL pg_hba.conf allows connections from Pi's IP

### Can't access application from other devices# 2. PostgreSQL is listening on the correct interface (postgresql.conf)

# 3. Firewall rules allow port 5432

```bash# 4. Network connectivity between Pi and DB server

# Ensure Docker is listening on all interfaces```

# In docker-compose.pi.yml, ports should be:

ports:### Migrations not running

  - "0.0.0.0:5099:8080"

The application automatically runs pending migrations on startup. If migrations fail:

# Check firewall on Raspberry Pi

sudo ufw status```bash

sudo ufw allow 5099/tcp# View detailed logs

docker-compose logs budgetexperiment | grep -i migration

# Verify container is running and healthy

docker compose -f docker-compose.pi.yml ps# Common causes:

```# 1. Database user lacks permissions (need CREATE, ALTER, DROP)

# 2. Database doesn't exist (create it first)

## Security Considerations# 3. Network connectivity issues

```

1. **Never expose PostgreSQL directly to the internet** - use a VPN or SSH tunnel

2. **Use strong passwords** in connection strings### Performance issues on Raspberry Pi

3. **Enable SSL/TLS** for database connections in production

4. **Keep the `.env` file secure** - restrict file permissions:```bash

   ```bash# Monitor resource usage

   chmod 600 .envdocker stats budgetexperiment

   ```

5. **Regularly update** - Pull latest images periodically# If high CPU/memory:

6. **Use reverse proxy** (nginx/Caddy) with HTTPS for production# 1. Reduce resource limits in docker-compose.yml

7. **Enable firewall** and restrict access to necessary ports only# 2. Optimize database queries

8. **Rotate GitHub tokens** periodically# 3. Consider upgrading to Pi 4 with more RAM

# 4. Use lighter database operations

## Backup and Restore```



The application is stateless - all data is in PostgreSQL. Backup strategy:### Can't access application from other devices



```bash```bash

# Backup PostgreSQL database# Ensure Docker is listening on all interfaces

pg_dump -h your-db-server -U your-username -d budgetexperiment > backup.sql# In docker-compose.yml, ports should be:

ports:

# Restore  - "0.0.0.0:5099:8080"

psql -h your-db-server -U your-username -d budgetexperiment < backup.sql

```# Check firewall on Raspberry Pi

sudo ufw status

## Monitoringsudo ufw allow 5099/tcp



### Health Checks# Verify container is running

docker-compose ps

The application includes a built-in health check at `/health`.```



```bash## Security Considerations

# Add to crontab for automated monitoring

*/5 * * * * curl -f http://localhost:5099/health || echo "App down!" | mail -s "Alert" you@example.com1. **Never expose PostgreSQL directly to the internet** - use a VPN or SSH tunnel

```2. **Use strong passwords** in connection strings

3. **Enable SSL/TLS** for database connections in production

### Docker Stats4. **Keep the `.env` file secure** - restrict file permissions:

   ```bash

```bash   chmod 600 .env

# Real-time resource monitoring   ```

docker stats budgetexperiment5. **Regularly update** the Docker images and application

```6. **Use reverse proxy** (nginx/Caddy) with HTTPS for production

7. **Enable firewall** and restrict access to necessary ports only

## Development Workflow

## Advanced: Multi-Architecture Builds

### Making Changes

If building on a non-ARM machine (e.g., development PC) and deploying to Raspberry Pi:

1. Make code changes locally

2. Test locally with `dotnet run````bash

3. Commit and push to GitHub# Set up Docker buildx for cross-platform builds

4. GitHub Actions automatically builds and publishes new imagedocker buildx create --name multiarch --driver docker-container --use

5. On Raspberry Pi, pull and deploy updated image:docker buildx inspect --bootstrap



```bash# Build for ARM64

docker compose -f docker-compose.pi.yml pulldocker buildx build --platform linux/arm64 -t budgetexperiment:latest --load .

docker compose -f docker-compose.pi.yml up -d

```# Or build and push to registry

docker buildx build --platform linux/arm64,linux/amd64 -t yourregistry/budgetexperiment:latest --push .

### Creating Releases```



To create a versioned release:## Backup and Restore



```bashThe application is stateless - all data is in PostgreSQL. Backup strategy:

# Tag a release

git tag -a v1.0.0 -m "Release version 1.0.0"```bash

git push origin v1.0.0# Backup PostgreSQL database

```pg_dump -h your-db-server -U your-username -d budgetexperiment > backup.sql



GitHub Actions will build and publish images with version tags.# Restore

psql -h your-db-server -U your-username -d budgetexperiment < backup.sql

## Support```



For issues or questions:## Monitoring

1. Check the logs: `docker compose -f docker-compose.pi.yml logs -f`

2. Review this guide's troubleshooting section### Simple monitoring with health checks

3. Check the main README.md for application-specific guidance

4. Review .github/copilot-instructions.md for architectural details```bash

# Add to crontab for automated health checks

## Quick Reference*/5 * * * * curl -f http://localhost:5099/health || echo "App down!" | mail -s "Alert" you@example.com

```

**Initial deployment:**

```bash### Docker stats

# 1. Authenticate

docker login ghcr.io -u YOUR_USERNAME -p YOUR_TOKEN```bash

# Real-time resource monitoring

# 2. Create directory and filesdocker stats budgetexperiment

mkdir -p ~/BudgetExperiment && cd ~/BudgetExperiment```

# (Download docker-compose.pi.yml and create .env)

## Support

# 3. Deploy

docker compose -f docker-compose.pi.yml up -dFor issues or questions:

```1. Check the logs: `docker-compose logs -f`

2. Review this guide's troubleshooting section

**Update deployment:**3. Check the main README.md for application-specific guidance

```bash4. Review .github/copilot-instructions.md for architectural details

cd ~/BudgetExperiment

docker compose -f docker-compose.pi.yml pull## Summary of Windows-to-Pi Deployment

docker compose -f docker-compose.pi.yml up -d

```### Quick Reference



**View logs:****Complete deployment (recommended):**

```bash```powershell

docker compose -f docker-compose.pi.yml logs -f# On Windows - builds, transfers, and deploys in one command

```.\deploy-to-pi.ps1 -PiHost raspberry-pi.local -PiUser pi

```

**Access application:**

- `http://raspberry-pi-ip:5099` - Web UI**Individual steps:**

- `http://raspberry-pi-ip:5099/scalar` - API Documentation```powershell

# Step 1: Build on Windows
.\build-docker-windows.ps1

# Step 2: Transfer and deploy
.\deploy-to-pi.ps1 -PiHost raspberry-pi.local -PiUser pi
```

**Files created:**
- `build-docker-windows.ps1` - Builds ARM64 Docker image on Windows
- `deploy-to-pi.ps1` - Complete deployment automation (build + transfer + deploy)
- `load-and-deploy.sh` - Runs on Pi to load image and start containers

**Prerequisites checklist:**
- [ ] Docker Desktop installed on Windows with buildx enabled
- [ ] SSH access to Raspberry Pi configured
- [ ] Database connection string ready
- [ ] `.env` file created on Raspberry Pi with `DB_CONNECTION_STRING`

**Access after deployment:**
- Application: `http://raspberry-pi.local:5099`
- API Docs: `http://raspberry-pi.local:5099/scalar`
- Health: `http://raspberry-pi.local:5099/health`

