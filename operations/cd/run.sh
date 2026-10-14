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
CORE_API_IMAGE_TAG="${REGISTRY_HOST}/template-core-api:${VERSION}"
API_IMAGE_TAG="${REGISTRY_HOST}/template-api:${VERSION}"
DOCS_IMAGE_TAG="${REGISTRY_HOST}/template-docs:${VERSION}"

TEMPLATE_NETWORK="template-network"
RP_NETWORK="rp_default_net"

CORE_API_DP_KEYS_VOLUME="template-core-api-dp-keys"
CORE_API_LOGS_VOLUME="template-core-api-logs"
API_DP_KEYS_VOLUME="template-api-dp-keys"
API_LOGS_VOLUME="template-api-logs"

COMPOSE_FILE="${SCRIPT_DIR}/docker-compose.yml"

if [ ! -f "${COMPOSE_FILE}" ]; then
  echo "Compose file not found: ${COMPOSE_FILE}"
  exit 1
fi

for image in "${CORE_API_IMAGE_TAG}" "${API_IMAGE_TAG}" "${DOCS_IMAGE_TAG}"; do
  if ! docker image inspect "${image}" >/dev/null 2>&1; then
    echo "Image not found locally: ${image}"
    echo "Pull it first: ./pull.sh ${VERSION}"
    exit 1
  fi
done

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

ensure_network "${TEMPLATE_NETWORK}"
ensure_network "${RP_NETWORK}"
ensure_volume "${CORE_API_DP_KEYS_VOLUME}"
ensure_volume "${CORE_API_LOGS_VOLUME}"
ensure_volume "${API_DP_KEYS_VOLUME}"
ensure_volume "${API_LOGS_VOLUME}"

echo "Recreating docker compose stack from ${COMPOSE_FILE}..."
docker compose -f "${COMPOSE_FILE}" down

# The merged compose includes all apps, which share the ${IMAGE_TAG} variable,
# so a single `up` cannot give them different image tags. Each app is therefore
# brought up on its own with its own IMAGE_TAG into the same compose project;
# container_name and env_file resolve from each included file's own defaults.
IMAGE_TAG="${CORE_API_IMAGE_TAG}" docker compose -f "${COMPOSE_FILE}" up -d core-api
IMAGE_TAG="${API_IMAGE_TAG}" docker compose -f "${COMPOSE_FILE}" up -d api
IMAGE_TAG="${DOCS_IMAGE_TAG}" docker compose -f "${COMPOSE_FILE}" up -d docs

echo "Stack is running."
echo "- Core API: ${CORE_API_IMAGE_TAG}"
echo "- Api:      ${API_IMAGE_TAG}"
echo "- Docs:     ${DOCS_IMAGE_TAG}"
echo "- Compose:  ${COMPOSE_FILE}"
