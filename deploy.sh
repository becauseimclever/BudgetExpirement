#!/bin/bash
# Deployment script for BudgetExperiment to Raspberry Pi
# Runs tests, builds Docker image, and deploys via docker-compose

set -e  # Exit on error

echo "========================================"
echo "BudgetExperiment Deployment Script"
echo "========================================"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Configuration
SOLUTION_PATH="/home/fortinbra/BudgetExpirement/BudgetExperiment.sln"
DOCKER_IMAGE_NAME="budgetexperiment"
DOCKER_IMAGE_TAG="latest"

# Function to print colored messages
print_info() {
    echo -e "${CYAN}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if running with sudo and exit if so (dotnet doesn't work well with sudo)
if [ "$EUID" -eq 0 ]; then 
    print_error "This script should NOT be run with sudo!"
    print_info "Run it as your regular user: ./deploy.sh"
    print_info "The script will prompt for sudo password only when needed for Docker commands."
    exit 1
fi

# Step 1: Run tests
print_info "Running all tests..."
if dotnet test "$SOLUTION_PATH" --configuration Release; then
    print_success "All tests passed!"
else
    print_error "Tests failed. Aborting deployment."
    exit 1
fi

# Step 2: Check for .env file with database connection string
print_info "Checking for .env file with database connection string..."
if [ ! -f ".env" ]; then
    print_warning ".env file not found!"
    echo ""
    echo "Please create a .env file with your database connection string:"
    echo "  DB_CONNECTION_STRING=Host=your-db-server;Port=5432;Database=budgetexperiment;Username=your-user;Password=your-password"
    echo ""
    read -p "Do you want to continue anyway? (y/N) " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        print_error "Deployment cancelled."
        exit 1
    fi
else
    print_success ".env file found"
fi

# Step 2.5: Build and publish the application locally
print_info "Building and publishing application locally for linux-arm64..."
API_PROJECT="/home/fortinbra/BudgetExpirement/src/BudgetExperiment.Api/BudgetExperiment.Api.csproj"
PUBLISH_DIR="/home/fortinbra/BudgetExpirement/publish"

# Clean previous publish
if [ -d "$PUBLISH_DIR" ]; then
    rm -rf "$PUBLISH_DIR"
fi

# Publish for ARM64 (Raspberry Pi)
if dotnet publish "$API_PROJECT" \
    --configuration Release \
    --runtime linux-arm64 \
    --self-contained false \
    --output "$PUBLISH_DIR" \
    /p:PublishTrimmed=false; then
    print_success "Application published successfully to $PUBLISH_DIR"
else
    print_error "Publishing failed. Aborting deployment."
    exit 1
fi

# Step 3: Build Docker image
print_info "Building Docker image for ARM64 (Raspberry Pi)..."
print_warning "You may be prompted for your sudo password for Docker commands..."
if sudo docker build -t "$DOCKER_IMAGE_NAME:$DOCKER_IMAGE_TAG" .; then
    print_success "Docker image built successfully!"
else
    print_error "Docker build failed. Aborting deployment."
    exit 1
fi

# Step 4: Show image info
print_info "Docker image details:"
sudo docker images "$DOCKER_IMAGE_NAME:$DOCKER_IMAGE_TAG"

# Step 5: Deploy with docker-compose
print_info "Deploying application with docker-compose..."
if sudo docker-compose up -d; then
    print_success "Application deployed successfully!"
else
    print_error "Deployment failed."
    exit 1
fi

# Step 6: Show container status
print_info "Container status:"
sudo docker-compose ps

# Step 7: Show logs (last 20 lines)
print_info "Recent logs:"
sudo docker-compose logs --tail=20

echo ""
print_success "========================================"
print_success "Deployment Complete!"
print_success "========================================"
echo ""
echo "Access the application at: http://localhost:5099"
echo ""
echo "Useful commands:"
echo "  View logs:        docker-compose logs -f"
echo "  Stop app:         docker-compose stop"
echo "  Restart app:      docker-compose restart"
echo "  Remove app:       docker-compose down"
echo "  View health:      curl http://localhost:5099/health"
echo "  View API docs:    http://localhost:5099/scalar"
echo ""
