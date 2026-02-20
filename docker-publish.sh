#!/bin/bash
CREDS_FILE=".ghcr_creds"
REGISTRY="ghcr.io"
IMAGE_NAME="lidarr"

# Setup check
if [ ! -f "$CREDS_FILE" ]; then
    echo "ERROR: Credential file '$CREDS_FILE' not found!"
    echo "Please create it with the following content:"
    echo "  GH_USER=your_username"
    echo "  GH_TOKEN=your_pat_token"
    exit 1
fi

if [ ! -f ".build_version" ]; then 
    echo "Missing .build_version, run build-assets.sh in DevContainer first!"; 
    exit 1; 
fi

# Determine version information
FULL_VERSION="$(cat .build_version)"

echo "Automatically determined version to be: ${FULL_VERSION}"

echo "Loading credentials from: $CREDS_FILE"
source "$CREDS_FILE"
if [ -z "$GH_USER" ] || [ -z "$GH_TOKEN" ]; then
    echo "ERROR: $CREDS_FILE is missing GH_USER or GH_TOKEN."
    exit 1
fi

# Login to github
echo "Logging into $REGISTRY as $GH_USER..."
echo "$GH_TOKEN" | docker login "$REGISTRY" -u "$GH_USER" --password-stdin
if [ $? -ne 0 ]; then
    echo "ERROR: Login failed. Check your token permissions."
    exit 1
fi

# Build docker image
LOWER_USER=$(echo "$GH_USER" | tr '[:upper:]' '[:lower:]')
IMAGE_URL="$REGISTRY/$LOWER_USER/$IMAGE_NAME"

echo "Building image: $IMAGE_URL:latest..."
docker build \
  --build-arg APP_VERSION="$FULL_VERSION" \
  -t "$IMAGE_URL:$FULL_VERSION" \
  -t "$IMAGE_URL:latest" \
  -f Dockerfile.prod .

if [ $? -ne 0 ]; then
    echo "ERROR: Docker build failed."
    exit 1
fi

# Push
echo "Pushing image to GHCR..."
docker push "$IMAGE_URL:latest"
echo "Successfully deployed to $IMAGE_URL"