#!/usr/bin/env sh
set -eu

if [ "$#" -ne 1 ]; then
  if [ "$#" -lt 1 ]; then
    echo "Missing required parameter: VERSION"
  else
    echo "Invalid parameter count: expected 1, got $#"
  fi
  echo "Usage: ./build.sh <version>"
  echo "Example: ./build.sh 1.0.0"
  exit 1
fi

VERSION="$1"

SCRIPT_DIR="$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)"
REPO_DIR="$(CDPATH= cd -- "${SCRIPT_DIR}/../../.." && pwd)"
REGISTRY_HOST="build.steubing.com"
IMAGE_NAME="acme-api"
IMAGE_TAG="${REGISTRY_HOST}/${IMAGE_NAME}:${VERSION}"

docker build \
  -t "${IMAGE_TAG}" \
  -f "${SCRIPT_DIR}/Dockerfile" \
  "${REPO_DIR}"

echo "Image built: ${IMAGE_TAG}"
