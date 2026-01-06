#!/bin/bash

# Build and push Docker images for linux/amd64 platform
# Usage: ./build-linuxamd64-images-and-push.sh [version]
# Default version: latest

VERSION=${1:-latest}
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SLN_FOLDER="$(cd "$SCRIPT_DIR/../../" && pwd)"

set -e  # Exit on error

echo "Building and pushing images with version: $VERSION"
echo "Solution folder: $SLN_FOLDER"

# Function to build and push an image
build_and_push() {
    local SERVICE_NAME=$1
    local PROJECT_PATH=$2
    local IMAGE_NAME=$3
    
    echo ""
    echo "********* BUILDING $SERVICE_NAME *********"
    cd "$PROJECT_PATH"
    
    echo "Publishing $SERVICE_NAME..."
    dotnet publish -c Release -o bin/Release/net10.0/publish
    
    # Wait for file system to sync
    sleep 1
    
    PUBLISH_PATH="$PROJECT_PATH/bin/Release/net10.0/publish"
    if [ ! -d "$PUBLISH_PATH" ]; then
        echo "ERROR: Publish folder not found: $PUBLISH_PATH"
        exit 1
    fi
    
    echo "Publish successful. Output: $PUBLISH_PATH"
    echo "Building Docker image for $SERVICE_NAME (linux/amd64)..."
    
    docker buildx build --platform linux/amd64 -f Dockerfile.local -t "$IMAGE_NAME:$VERSION" . --push
    
    if [ $? -eq 0 ]; then
        echo "Docker image built and pushed successfully for $SERVICE_NAME"
    else
        echo "ERROR: Docker build failed for $SERVICE_NAME"
        exit 1
    fi
}

# Build and push Blazor
build_and_push "Blazor" \
    "$SLN_FOLDER/src/HC.Blazor" \
    "longnguyen1331/hc-blazor"

# Build and push API
build_and_push "Api.Host" \
    "$SLN_FOLDER/src/HC.HttpApi.Host" \
    "longnguyen1331/hc-api"

# Build and push AuthServer
build_and_push "AuthServer" \
    "$SLN_FOLDER/src/HC.AuthServer" \
    "longnguyen1331/hc-authserver"

echo ""
echo "********* ALL COMPLETED *********"
echo "All images have been built and pushed successfully!"

