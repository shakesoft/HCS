#!/bin/bash

# Script to force pull latest images and restart containers
# Usage: ./update-and-restart.sh [compose-file]
# Default: docker-compose-dev-port10.65.37.105.yml

COMPOSE_FILE=${1:-docker-compose-dev-port10.65.37.105.yml}
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

set -e  # Exit on error

echo "=========================================="
echo "Updating and Restarting Containers"
echo "=========================================="
echo "Compose file: $COMPOSE_FILE"
echo ""

cd "$SCRIPT_DIR"

# Step 1: Stop and remove containers
echo "Step 1: Stopping and removing containers..."
docker-compose -f "$COMPOSE_FILE" down

# Step 2: Remove old images to force pull new ones
echo ""
echo "Step 2: Removing old images to force pull..."
docker rmi longnguyen1331/hc-blazor:latest 2>/dev/null || echo "  hc-blazor:latest not found (will pull new)"
docker rmi longnguyen1331/hc-api:latest 2>/dev/null || echo "  hc-api:latest not found (will pull new)"
docker rmi longnguyen1331/hc-authserver:latest 2>/dev/null || echo "  hc-authserver:latest not found (will pull new)"
docker rmi longnguyen1331/hc-db-migrator:latest 2>/dev/null || echo "  hc-db-migrator:latest not found (will pull new)"

# Step 3: Force pull latest images
echo ""
echo "Step 3: Pulling latest images..."
docker-compose -f "$COMPOSE_FILE" pull --ignore-pull-failures

# Step 4: Build images if needed (for local builds)
echo ""
echo "Step 4: Building images (if needed)..."
docker-compose -f "$COMPOSE_FILE" build --pull || echo "  Build skipped (using pre-built images)"

# Step 5: Start containers
echo ""
echo "Step 5: Starting containers..."
docker-compose -f "$COMPOSE_FILE" up -d

# Step 6: Show status
echo ""
echo "Step 6: Container status..."
docker-compose -f "$COMPOSE_FILE" ps

echo ""
echo "=========================================="
echo "Update completed!"
echo "=========================================="
echo ""
echo "To view logs:"
echo "  docker-compose -f $COMPOSE_FILE logs -f"
echo ""
echo "To check specific service:"
echo "  docker logs hc-blazor --tail 50"
echo "  docker logs hc-api --tail 50"
echo "  docker logs hc-authserver --tail 50"

