#!/usr/bin/env sh
set -eu

if [ "$#" -ne 1 ]; then
  if [ "$#" -lt 1 ]; then
    echo "Missing required parameter: VERSION"
  else
    echo "Invalid parameter count: expected 1, got $#"
  fi
  echo "Usage: ./pull.sh <version>"
  echo "Example: ./pull.sh 1.0.0"
  exit 1
fi

VERSION="$1"

REGISTRY_HOST="build.steubing.com"
IMAGE_NAME="wst-trios-core-api"
IMAGE_TAG="${REGISTRY_HOST}/${IMAGE_NAME}:${VERSION}"

echo "Pulling ${IMAGE_TAG} from ${REGISTRY_HOST}..."
echo "If pull fails with 'unauthorized', run: docker login ${REGISTRY_HOST}"
docker pull "${IMAGE_TAG}"

echo "Image pulled: ${IMAGE_TAG}"
