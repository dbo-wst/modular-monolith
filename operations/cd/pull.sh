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
SCRIPT_DIR="$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)"

chmod +x "${SCRIPT_DIR}/core-api/pull.sh"
chmod +x "${SCRIPT_DIR}/api/pull.sh"
chmod +x "${SCRIPT_DIR}/docs/pull.sh"

"${SCRIPT_DIR}/core-api/pull.sh" "${VERSION}"
"${SCRIPT_DIR}/api/pull.sh" "${VERSION}"
"${SCRIPT_DIR}/docs/pull.sh" "${VERSION}"
