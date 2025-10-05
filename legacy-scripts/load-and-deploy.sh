#!/bin/bash
# Load Docker image and deploy with docker-compose on Raspberry Pi
# This script is designed to run ON the Raspberry Pi after files are transferred

set -e  # Exit on error

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${CYAN}=======================================${NC}"
echo -e "${CYAN}Loading Docker Image and Deploying${NC}"
echo -e "${CYAN}=======================================${NC}"
echo ""

# Configuration
IMAGE_FILE="budgetexperiment-image.tar"
IMAGE_NAME="budgetexperiment:latest"

# Step 1: Load Docker image
echo -e "${YELLOW}[1/4] Loading Docker image...${NC}"
if [ ! -f "$IMAGE_FILE" ]; then
    echo -e "${RED}✗ Error: $IMAGE_FILE not found!${NC}"
    echo "Please ensure the image file was transferred successfully."
    exit 1
fi

if docker load -i "$IMAGE_FILE"; then
    echo -e "${GREEN}✓ Docker image loaded successfully!${NC}"
else
    echo -e "${RED}✗ Failed to load Docker image.${NC}"
    exit 1
fi
echo ""

# Step 2: Verify image
echo -e "${YELLOW}[2/4] Verifying Docker image...${NC}"
docker images budgetexperiment
echo ""

# Step 3: Check for .env file
echo -e "${YELLOW}[3/4] Checking for .env file...${NC}"
if [ ! -f ".env" ]; then
    echo -e "${YELLOW}⚠ Warning: .env file not found!${NC}"
    echo ""
    echo "Please create a .env file with your database connection string:"
    echo "  DB_CONNECTION_STRING=Host=your-db-server;Port=5432;Database=budgetexperiment;Username=your-user;Password=your-password"
    echo ""
    read -p "Do you want to continue anyway? (y/N) " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        echo -e "${RED}Deployment cancelled.${NC}"
        exit 1
    fi
else
    echo -e "${GREEN}✓ .env file found${NC}"
fi
echo ""

# Step 4: Deploy with docker-compose
echo -e "${YELLOW}[4/4] Deploying with docker-compose...${NC}"

# Stop existing containers if running
if docker-compose ps | grep -q budgetexperiment; then
    echo "Stopping existing containers..."
    docker-compose down
fi

# Start new container
if docker-compose up -d; then
    echo -e "${GREEN}✓ Application deployed successfully!${NC}"
else
    echo -e "${RED}✗ Deployment failed.${NC}"
    exit 1
fi
echo ""

# Step 5: Show status
echo -e "${YELLOW}Container status:${NC}"
docker-compose ps
echo ""

# Wait a moment for app to start
echo "Waiting for application to start..."
sleep 5

# Step 6: Show recent logs
echo -e "${YELLOW}Recent logs:${NC}"
docker-compose logs --tail=30
echo ""

# Step 7: Clean up tar file
echo "Cleaning up image tar file..."
rm -f "$IMAGE_FILE"
echo ""

echo -e "${GREEN}=======================================${NC}"
echo -e "${GREEN}Deployment Complete!${NC}"
echo -e "${GREEN}=======================================${NC}"
echo ""
echo "Access the application at: http://localhost:5099"
echo ""
echo "Useful commands:"
echo "  docker-compose logs -f        # Follow logs"
echo "  docker-compose ps             # Check status"
echo "  docker-compose restart        # Restart app"
echo "  docker-compose down           # Stop app"
echo "  curl http://localhost:5099/health   # Check health"
echo ""
