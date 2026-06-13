#!/usr/bin/env bash
set -euo pipefail

REPO_URL="${REPO_URL:-https://github.com/rizkisetyap/ai-trader-botv2.git}"
BRANCH="${BRANCH:-main}"
DEPLOY_DIR="${DEPLOY_DIR:-$HOME/ai-trader-bot}"

if [ ! -d "$DEPLOY_DIR/.git" ]; then
  rm -rf "$DEPLOY_DIR"
  git clone --branch "$BRANCH" "$REPO_URL" "$DEPLOY_DIR"
fi

cd "$DEPLOY_DIR"
git fetch origin "$BRANCH"
git reset --hard "origin/$BRANCH"
docker compose up -d --build