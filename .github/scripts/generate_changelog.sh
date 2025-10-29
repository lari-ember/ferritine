#!/usr/bin/env bash
# .github/scripts/generate_changelog.sh
#
# Script para gerar changelog entre duas tags do Git.
#
# Este script cria um fragmento de CHANGELOG.md com todos os commits
# entre a tag anterior e a nova tag especificada.
#
# Uso:
#   ./generate_changelog.sh <NEW_TAG>
#
# Exemplo:
#   ./generate_changelog.sh v0.2.0
#
# O script irá:
# 1. Encontrar a tag anterior à tag especificada
# 2. Listar todos os commits entre essas tags
# 3. Gerar um changelog formatado com data e lista de commits
#
set -euo pipefail

NEW_TAG="$1"

# Try to find previous tag (the tag before NEW_TAG)
# List tags sorted by version (descending), find NEW_TAG position and take next one.
TAGS=($(git tag --sort=-v:refname))
PREV_TAG=""
for i in "${!TAGS[@]}"; do
  if [ "${TAGS[$i]}" = "${NEW_TAG}" ]; then
    # previous tag is next array entry (because sorted desc)
    if [ $((i+1)) -lt "${#TAGS[@]}" ]; then
      PREV_TAG="${TAGS[$((i+1))]}"
    fi
    break
  fi
done

if [ -z "${PREV_TAG}" ]; then
  # If no previous tag found, take all history
  RANGE=""
else
  RANGE="${PREV_TAG}..${NEW_TAG}"
fi

# Compose changelog header
DATE=$(date -I)
echo "## ${NEW_TAG} - ${DATE}" > changelog_fragment.md
echo "" >> changelog_fragment.md

if [ -z "${RANGE}" ]; then
  git log --pretty=format:'- %s (%h)' >> changelog_fragment.md
else
  git log "${RANGE}" --pretty=format:'- %s (%h)' >> changelog_fragment.md
fi

# Output path
cat changelog_fragment.md

