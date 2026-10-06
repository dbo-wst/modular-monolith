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
SCRIPT_DIR="$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)"

echo "==> Pushing core-api (${VERSION})"
chmod +x "${SCRIPT_DIR}/core-api/push.sh"
"${SCRIPT_DIR}/core-api/push.sh" "${VERSION}"

echo "==> Pushing api (${VERSION})"
chmod +x "${SCRIPT_DIR}/api/push.sh"
"${SCRIPT_DIR}/api/push.sh" "${VERSION}"

echo "==> Pushing docs (${VERSION})"
chmod +x "${SCRIPT_DIR}/docs/push.sh"
"${SCRIPT_DIR}/docs/push.sh" "${VERSION}"
