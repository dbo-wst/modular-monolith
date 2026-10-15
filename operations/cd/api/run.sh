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
IMAGE_NAME="acme-api"
IMAGE_TAG="${REGISTRY_HOST}/${IMAGE_NAME}:${VERSION}"

CONTAINER_NAME="acme-api"
ACME_NETWORK="acme-network"
RP_NETWORK="rp_default_net"
DP_KEYS_VOLUME="acme-api-dp-keys"
LOGS_VOLUME="acme-api-logs"

COMPOSE_FILE="${SCRIPT_DIR}/docker-compose.yml"
ENV_FILE_PATH="${SCRIPT_DIR}/conf.d/.env"

if [ ! -f "${COMPOSE_FILE}" ]; then
  echo "Compose file not found: ${COMPOSE_FILE}"
  exit 1
fi

if [ ! -f "${ENV_FILE_PATH}" ]; then
  echo "Env file not found: ${ENV_FILE_PATH}"
  echo "Create it from a template, e.g.: cp ${SCRIPT_DIR}/conf.d/.env.dev ${ENV_FILE_PATH}"
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

ensure_volume() {
  name="$1"
  if ! docker volume inspect "${name}" >/dev/null 2>&1; then
    echo "Creating docker volume: ${name}"
    docker volume create "${name}" >/dev/null
  fi
}

ensure_network "${ACME_NETWORK}"
ensure_network "${RP_NETWORK}"
ensure_volume "${DP_KEYS_VOLUME}"
ensure_volume "${LOGS_VOLUME}"

echo "Recreating docker compose stack from ${COMPOSE_FILE}..."
IMAGE_TAG="${IMAGE_TAG}" \
CONTAINER_NAME="${CONTAINER_NAME}" \
ENV_FILE="${ENV_FILE_PATH}" \
docker compose -f "${COMPOSE_FILE}" down

IMAGE_TAG="${IMAGE_TAG}" \
CONTAINER_NAME="${CONTAINER_NAME}" \
ENV_FILE="${ENV_FILE_PATH}" \
docker compose -f "${COMPOSE_FILE}" up -d

echo "Container is running."
echo "- Name: ${CONTAINER_NAME}"
echo "- Image: ${IMAGE_TAG}"
echo "- Env:   ${ENV_FILE_PATH}"
