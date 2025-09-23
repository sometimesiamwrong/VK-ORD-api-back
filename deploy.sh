#!/bin/bash

# VK ORD API Wrapper - Deployment Script
# This script helps deploy the application to production

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Configuration
COMPOSE_FILE="docker-compose.yml"
ENV_FILE=".env"

echo -e "${GREEN}VK ORD API Wrapper - Production Deployment${NC}"
echo "=============================================="

# Check if .env file exists
if [ ! -f "$ENV_FILE" ]; then
    echo -e "${RED}Error: $ENV_FILE file not found!${NC}"
    echo -e "${YELLOW}Please create $ENV_FILE file based on .env.example${NC}"
    echo -e "${YELLOW}Required environment variables:${NC}"
    echo "  - DB_PASSWORD"
    echo "  - JWT_SECRET_KEY"
    echo "  - OPENROUTER_API_KEY"
    echo "  - DADATA_API_TOKEN"
    exit 1
fi

# Check if docker and docker-compose are installed
if ! command -v docker &> /dev/null; then
    echo -e "${RED}Error: Docker is not installed!${NC}"
    exit 1
fi

if ! command -v docker-compose &> /dev/null && ! docker compose version &> /dev/null; then
    echo -e "${RED}Error: Docker Compose is not installed!${NC}"
    exit 1
fi

# Function to use docker compose (new or old syntax)
docker_compose_cmd() {
    if docker compose version &> /dev/null; then
        docker compose "$@"
    else
        docker-compose "$@"
    fi
}

# Ask for deployment action
echo -e "${YELLOW}Select deployment action:${NC}"
echo "1) Build and start (fresh deployment)"
echo "2) Update application only"
echo "3) Restart all services"
echo "4) Stop and remove all services"
echo "5) View logs"
echo "6) Show status"
read -p "Enter your choice (1-6): " choice

case $choice in
    1)
        echo -e "${GREEN}Starting fresh deployment...${NC}"
        echo -e "${YELLOW}This will build the application and start all services${NC}"

        # Stop and remove existing containers
        echo "Stopping existing services..."
        docker_compose_cmd down -v 2>/dev/null || true

        # Build and start services
        echo "Building and starting services..."
        docker_compose_cmd up --build -d

        # Wait for services to be healthy
        echo "Waiting for services to start..."
        sleep 10

        # Show status
        docker_compose_cmd ps
        ;;
    2)
        echo -e "${GREEN}Updating application only...${NC}"

        # Build new image
        docker_compose_cmd build vkord-api

        # Restart application
        docker_compose_cmd up -d vkord-api

        echo "Application updated successfully"
        ;;
    3)
        echo -e "${GREEN}Restarting all services...${NC}"
        docker_compose_cmd restart
        ;;
    4)
        echo -e "${GREEN}Stopping and removing all services...${NC}"
        docker_compose_cmd down -v
        echo "All services stopped and removed"
        ;;
    5)
        echo -e "${GREEN}Showing logs...${NC}"
        echo "Press Ctrl+C to exit logs"
        docker_compose_cmd logs -f
        ;;
    6)
        echo -e "${GREEN}Service status:${NC}"
        docker_compose_cmd ps
        ;;
    *)
        echo -e "${RED}Invalid choice!${NC}"
        exit 1
        ;;
esac

echo -e "${GREEN}Deployment script completed!${NC}"

# Show useful information
if [ "$choice" = "1" ] || [ "$choice" = "2" ]; then
    echo ""
    echo -e "${YELLOW}Useful commands:${NC}"
    echo "  • View logs: docker-compose logs -f"
    echo "  • Check status: docker-compose ps"
    echo "  • Restart app: docker-compose restart vkord-api"
    echo "  • Access database: docker-compose exec postgres psql -U vkord_user -d vkord"
    echo ""
    echo -e "${YELLOW}Application URLs:${NC}"
    echo "  • API: http://localhost:8080"
    echo "  • Health check: http://localhost:8080/health"
    if docker_compose_cmd ps | grep -q nginx; then
        echo "  • Nginx proxy: http://localhost"
    fi
fi

