# Docker Deployment Guide for BudgetExperiment

This guide covers deploying the BudgetExperiment application to a Raspberry Pi using Docker.

## Prerequisites

- Raspberry Pi (3B+ or newer recommended) running a 64-bit OS
- Docker and Docker Compose installed on the Raspberry Pi
- PostgreSQL database server (running separately)
- Network access from Raspberry Pi to PostgreSQL server

## Quick Start

### 1. Install Docker on Raspberry Pi

If Docker is not already installed on your Raspberry Pi:

```bash
# Update system
sudo apt-get update && sudo apt-get upgrade -y

# Install Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# Add current user to docker group (avoid sudo)
sudo usermod -aG docker $USER

# Install Docker Compose
sudo apt-get install -y docker-compose

# Verify installation
docker --version
docker-compose --version
```

Log out and back in for group changes to take effect.

### 2. Configure Database Connection

Create a `.env` file with your PostgreSQL connection string:

```bash
# Copy the example file
cp .env.example .env

# Edit with your actual connection details
nano .env
```

Example `.env` content:

```env
DB_CONNECTION_STRING=Host=192.168.1.100;Port=5432;Database=budgetexperiment;Username=budgetuser;Password=YourSecurePassword123!
```

**Important**: Never commit `.env` to version control! It's already in `.gitignore`.

### 3. Deploy the Application

#### Option A: Using the deployment script (recommended)

```bash
# Make the script executable
chmod +x deploy.sh

# Run the deployment
./deploy.sh
```

This script will:
1. Run all tests
2. Build the Docker image for ARM64
3. Deploy the application using docker-compose
4. Show logs and status

#### Option B: Manual deployment

```bash
# Run tests
dotnet test BudgetExperiment.sln --configuration Release

# Build Docker image
docker build -t budgetexperiment:latest .

# Start the application
docker-compose up -d
```

### 4. Verify Deployment

```bash
# Check container status
docker-compose ps

# View logs
docker-compose logs -f

# Test health endpoint
curl http://localhost:5099/health

# Access the application
# Open browser: http://your-raspberry-pi-ip:5099
```

## Application URLs

Once deployed, the application is accessible at:

- **Web UI**: `http://your-raspberry-pi-ip:5099`
- **API Documentation (Scalar)**: `http://your-raspberry-pi-ip:5099/scalar`
- **OpenAPI Spec**: `http://your-raspberry-pi-ip:5099/openapi/v1.json`
- **Health Check**: `http://your-raspberry-pi-ip:5099/health`

## Management Commands

### View Logs
```bash
# Follow logs in real-time
docker-compose logs -f

# View last 50 lines
docker-compose logs --tail=50

# View specific service logs
docker-compose logs -f budgetexperiment
```

### Stop/Start/Restart
```bash
# Stop the application
docker-compose stop

# Start the application
docker-compose start

# Restart the application
docker-compose restart

# Stop and remove containers (but keep data)
docker-compose down

# Stop, remove containers, and remove images
docker-compose down --rmi all
```

### Update Deployment
```bash
# After code changes, rebuild and redeploy
docker-compose down
docker build -t budgetexperiment:latest .
docker-compose up -d
```

Or simply run:
```bash
./deploy.sh
```

## Configuration

### Database Connection String Format

The connection string follows Npgsql format:

```
Host=hostname;Port=5432;Database=dbname;Username=user;Password=pass;[options]
```

Common options:
- `SSL Mode=Require` - Force SSL connection
- `SSL Mode=Prefer` - Prefer SSL if available
- `Pooling=true` - Enable connection pooling (default)
- `Minimum Pool Size=1` - Minimum connections in pool
- `Maximum Pool Size=20` - Maximum connections in pool
- `Command Timeout=30` - Command timeout in seconds

### Port Configuration

By default, the application runs on port 5099. To change:

1. Edit `docker-compose.yml`:
   ```yaml
   ports:
     - "YOUR_PORT:8080"
   ```

2. Or set in `.env`:
   ```env
   BUDGET_APP_PORT=8080
   ```

### Resource Limits

Default resource limits (adjust in `docker-compose.yml` based on your Pi model):

- **CPU Limit**: 2 cores
- **Memory Limit**: 1GB
- **Memory Reservation**: 256MB

For Raspberry Pi 3B (1GB RAM):
```yaml
deploy:
  resources:
    limits:
      cpus: '2'
      memory: 512M
    reservations:
      cpus: '0.25'
      memory: 128M
```

For Raspberry Pi 4+ (4GB+ RAM): Current defaults are fine.

## Troubleshooting

### Container fails to start

```bash
# Check container logs
docker-compose logs budgetexperiment

# Check for common issues:
# 1. Database connection - verify connection string in .env
# 2. Port conflicts - ensure port 5099 is not in use
# 3. Resource limits - adjust if Raspberry Pi is constrained
```

### Database connection errors

```bash
# Test database connectivity from Raspberry Pi
# Install PostgreSQL client if needed
sudo apt-get install -y postgresql-client

# Test connection (replace with your details)
psql -h your-db-server -U your-username -d budgetexperiment

# If connection fails, check:
# 1. PostgreSQL pg_hba.conf allows connections from Pi's IP
# 2. PostgreSQL is listening on the correct interface (postgresql.conf)
# 3. Firewall rules allow port 5432
# 4. Network connectivity between Pi and DB server
```

### Migrations not running

The application automatically runs pending migrations on startup. If migrations fail:

```bash
# View detailed logs
docker-compose logs budgetexperiment | grep -i migration

# Common causes:
# 1. Database user lacks permissions (need CREATE, ALTER, DROP)
# 2. Database doesn't exist (create it first)
# 3. Network connectivity issues
```

### Performance issues on Raspberry Pi

```bash
# Monitor resource usage
docker stats budgetexperiment

# If high CPU/memory:
# 1. Reduce resource limits in docker-compose.yml
# 2. Optimize database queries
# 3. Consider upgrading to Pi 4 with more RAM
# 4. Use lighter database operations
```

### Can't access application from other devices

```bash
# Ensure Docker is listening on all interfaces
# In docker-compose.yml, ports should be:
ports:
  - "0.0.0.0:5099:8080"

# Check firewall on Raspberry Pi
sudo ufw status
sudo ufw allow 5099/tcp

# Verify container is running
docker-compose ps
```

## Security Considerations

1. **Never expose PostgreSQL directly to the internet** - use a VPN or SSH tunnel
2. **Use strong passwords** in connection strings
3. **Enable SSL/TLS** for database connections in production
4. **Keep the `.env` file secure** - restrict file permissions:
   ```bash
   chmod 600 .env
   ```
5. **Regularly update** the Docker images and application
6. **Use reverse proxy** (nginx/Caddy) with HTTPS for production
7. **Enable firewall** and restrict access to necessary ports only

## Advanced: Multi-Architecture Builds

If building on a non-ARM machine (e.g., development PC) and deploying to Raspberry Pi:

```bash
# Set up Docker buildx for cross-platform builds
docker buildx create --name multiarch --driver docker-container --use
docker buildx inspect --bootstrap

# Build for ARM64
docker buildx build --platform linux/arm64 -t budgetexperiment:latest --load .

# Or build and push to registry
docker buildx build --platform linux/arm64,linux/amd64 -t yourregistry/budgetexperiment:latest --push .
```

## Backup and Restore

The application is stateless - all data is in PostgreSQL. Backup strategy:

```bash
# Backup PostgreSQL database
pg_dump -h your-db-server -U your-username -d budgetexperiment > backup.sql

# Restore
psql -h your-db-server -U your-username -d budgetexperiment < backup.sql
```

## Monitoring

### Simple monitoring with health checks

```bash
# Add to crontab for automated health checks
*/5 * * * * curl -f http://localhost:5099/health || echo "App down!" | mail -s "Alert" you@example.com
```

### Docker stats

```bash
# Real-time resource monitoring
docker stats budgetexperiment
```

## Support

For issues or questions:
1. Check the logs: `docker-compose logs -f`
2. Review this guide's troubleshooting section
3. Check the main README.md for application-specific guidance
4. Review .github/copilot-instructions.md for architectural details
