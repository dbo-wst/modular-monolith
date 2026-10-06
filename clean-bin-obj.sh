#!/usr/bin/env bash

set -euo pipefail

root_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

mapfile -d '' directories < <(
  find "$root_dir" -type d \( -name bin -o -name obj \) -print0 | awk -v RS='\0' '{ print length, $0 }' | sort -rn | cut -d' ' -f2- | tr '\n' '\0'
)

if [ "${#directories[@]}" -eq 0 ]; then
  echo "No bin or obj directories found."
  exit 0
fi

for directory in "${directories[@]}"; do
  [ -n "$directory" ] || continue
  if [ -d "$directory" ]; then
    echo "Removing $directory"
    rm -rf -- "$directory"
  fi
done

echo "Cleanup complete."
