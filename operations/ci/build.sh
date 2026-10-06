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

echo "==> Building core-api (${VERSION})"
chmod +x "${SCRIPT_DIR}/core-api/build.sh"
"${SCRIPT_DIR}/core-api/build.sh" "${VERSION}"

echo "==> Building api (${VERSION})"
chmod +x "${SCRIPT_DIR}/api/build.sh"
"${SCRIPT_DIR}/api/build.sh" "${VERSION}"

echo "==> Building docs (${VERSION})"
chmod +x "${SCRIPT_DIR}/docs/build.sh"
"${SCRIPT_DIR}/docs/build.sh" "${VERSION}"
