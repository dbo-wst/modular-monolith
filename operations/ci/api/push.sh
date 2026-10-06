#!/usr/bin/env sh
set -eu

if [ "$#" -ne 1 ]; then
  if [ "$#" -lt 1 ]; then
    echo "Missing required parameter: VERSION"
  else
    echo "Invalid parameter count: expected 1, got $#"
  fi
  echo "Usage: ./push.sh <version>"
  echo "Example: ./push.sh 1.0.0"
  exit 1
fi

VERSION="$1"

REGISTRY_HOST="build.steubing.com"
IMAGE_NAME="wst-trios-api"
IMAGE_TAG="${REGISTRY_HOST}/${IMAGE_NAME}:${VERSION}"

if ! docker image inspect "${IMAGE_TAG}" >/dev/null 2>&1; then
  echo "Docker image not found: ${IMAGE_TAG}"
  echo "Build it first: ./build.sh ${VERSION}"
  exit 1
fi

echo "Pushing ${IMAGE_TAG} to ${REGISTRY_HOST}..."
echo "If push fails with 'unauthorized', run: docker login ${REGISTRY_HOST}"
docker push "${IMAGE_TAG}"

echo "Image pushed: ${IMAGE_TAG}"
