#!/bin/bash
set -e

# Determine version information
UPSTREAM_TAG=$(git ls-remote --tags --refs --sort='v:refname' https://github.com/Lidarr/Lidarr.git 'v*.*.*.*' | tail -n1 | sed 's/.*\/v//')
if [ -z "$UPSTREAM_TAG" ]; then
    echo "Could not reach upstream for versioning."
    exit 1
fi

HAL_REV=$(git rev-list --count upstream/develop..HEAD)
FULL_VERSION="${UPSTREAM_TAG}-hal-${HAL_REV}"

# Build production
echo "Building Frontend..."
export REACT_APP_APP_VERSION="$FULL_VERSION"
cd frontend
yarn install && yarn build --env production
cd ..

echo "Building Backend..."
dotnet publish src/Lidarr.sln \
  -p:TargetFramework=net8.0 \
  -p:GenerateFullPaths=true \
  -p:Version="$FULL_VERSION" \
  -p:InformationalVersion="$FULL_VERSION" \
  -c Release \
  -r linux-musl-x64 \
  --self-contained false \
  -o ./_output/net8.0 \
  "-p:ConsoleLoggerParameters=NoSummary"

echo "Linking frontend..."
ln -sr _output/UI _output/net8.0/UI

# Dump full version to file
echo "$FULL_VERSION" > .build_version