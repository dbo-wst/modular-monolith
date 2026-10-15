#!/usr/bin/env sh
set -eu

if [ "$#" -ne 1 ]; then
  if [ "$#" -lt 1 ]; then
    echo "Missing required parameter: VERSION"
  else
    echo "Invalid parameter count: expected 1, got $#"
  fi
  echo "Usage: ./run.sh <version>"
  echo "Example: ./run.sh 1.0.0"
  exit 1
fi

VERSION="$1"

SCRIPT_DIR="$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)"

REGISTRY_HOST="build.steubing.com"
IMAGE_NAME="acme-docs"
IMAGE_TAG="${REGISTRY_HOST}/${IMAGE_NAME}:${VERSION}"

CONTAINER_NAME="acme-docs"
ACME_NETWORK="acme-network"
RP_NETWORK="rp_default_net"

COMPOSE_FILE="${SCRIPT_DIR}/docker-compose.yml"

if [ ! -f "${COMPOSE_FILE}" ]; then
  echo "Compose file not found: ${COMPOSE_FILE}"
  exit 1
fi

if ! docker image inspect "${IMAGE_TAG}" >/dev/null 2>&1; then
  echo "Image not found locally: ${IMAGE_TAG}"
  echo "Pull it first: ./pull.sh ${VERSION}"
  exit 1
fi

ensure_network() {
  name="$1"
  if ! docker network inspect "${name}" >/dev/null 2>&1; then
    echo "Creating docker network: ${name}"
    docker network create "${name}" >/dev/null
  fi
}

ensure_network "${ACME_NETWORK}"
ensure_network "${RP_NETWORK}"

echo "Recreating docker compose stack from ${COMPOSE_FILE}..."
IMAGE_TAG="${IMAGE_TAG}" \
CONTAINER_NAME="${CONTAINER_NAME}" \
docker compose -f "${COMPOSE_FILE}" down

IMAGE_TAG="${IMAGE_TAG}" \
CONTAINER_NAME="${CONTAINER_NAME}" \
docker compose -f "${COMPOSE_FILE}" up -d

echo "Container is running."
echo "- Name: ${CONTAINER_NAME}"
echo "- Image: ${IMAGE_TAG}"
