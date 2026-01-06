#!/bin/bash

# Simple script to force pull latest images and restart
# Run this on Linux server after images are pushed

COMPOSE_FILE="docker-compose-dev-port10.65.37.105.yml"

echo "=========================================="
echo "Force Pull Latest Images and Restart"
echo "=========================================="

# Stop containers
echo "Stopping containers..."
docker-compose -f "$COMPOSE_FILE" down

# Remove old images to force pull
echo "Removing old images..."
docker rmi longnguyen1331/hc-blazor:latest longnguyen1331/hc-api:latest longnguyen1331/hc-authserver:latest longnguyen1331/hc-db-migrator:latest 2>/dev/null || true

# Pull latest images
echo "Pulling latest images..."
docker-compose -f "$COMPOSE_FILE" pull

# Start with force recreate
echo "Starting containers with force recreate..."
docker-compose -f "$COMPOSE_FILE" up -d --force-recreate

# Show status
echo ""
echo "Container status:"
docker-compose -f "$COMPOSE_FILE" ps

echo ""
echo "Done! Check logs with: docker-compose -f $COMPOSE_FILE logs -f"

