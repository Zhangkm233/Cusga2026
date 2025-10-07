#!/usr/bin/env bash
set -euo pipefail

# One-click push to GitHub
# Defaults:
# - remote: origin
# - branch: current branch (creates main if none)
# - message: "chore: update <date>"
# If no remote is configured, provide repo URL via --url or env GIT_REMOTE_URL.

script_dir="$(cd "$(dirname "$0")" && pwd)"
cd "$script_dir"

remote_name="origin"
commit_msg="${COMMIT_MSG:-chore: update $(date +'%Y-%m-%d %H:%M:%S')}"
repo_url="${GIT_REMOTE_URL:-}"
explicit_branch="${GIT_BRANCH:-}"

usage() {
  echo "Usage: $0 [--url <git_repo_url>] [--remote <name>] [--message <msg>] [--branch <branch>]"
  echo "Env vars: GIT_REMOTE_URL, COMMIT_MSG, GIT_BRANCH"
}

while [[ ${1:-} != "" ]]; do
  case "$1" in
    --url)
      shift; repo_url="${1:-}" ;;
    --remote)
      shift; remote_name="${1:-}" ;;
    --message|--msg|-m)
      shift; commit_msg="${1:-}" ;;
    --branch|-b)
      shift; explicit_branch="${1:-}" ;;
    -h|--help)
      usage; exit 0 ;;
    *)
      echo "Unknown option: $1"; usage; exit 1 ;;
  esac
  shift || true
done

if [[ ! -d .git ]]; then
  echo "Initializing new git repository..."
  git init
fi

# Determine branch
current_branch=""
if git rev-parse --git-dir >/dev/null 2>&1; then
  if current_branch=$(git rev-parse --abbrev-ref HEAD 2>/dev/null); then
    :
  else
    current_branch=""
  fi
fi

if [[ -n "$explicit_branch" ]]; then
  branch="$explicit_branch"
else
  branch="$current_branch"
fi

if [[ -z "$branch" || "$branch" == "HEAD" ]]; then
  branch="main"
  echo "Creating default branch '$branch'..."
  # Ensure branch exists for new repos without commits
  if ! git show-ref --verify --quiet "refs/heads/$branch"; then
    git checkout -B "$branch"
  fi
fi

# Remote setup
have_remote=false
if git remote get-url "$remote_name" >/dev/null 2>&1; then
  have_remote=true
else
  if [[ -n "$repo_url" ]]; then
    echo "Adding remote '$remote_name' -> $repo_url"
    git remote add "$remote_name" "$repo_url" || git remote set-url "$remote_name" "$repo_url"
    have_remote=true
  fi
fi

if [[ "$have_remote" != true ]]; then
  echo "No remote configured. Provide --url <git_repo_url> or set GIT_REMOTE_URL."
  exit 2
fi

# Stage and commit if needed
git add -A
if ! git diff --cached --quiet; then
  git commit -m "$commit_msg"
else
  echo "No changes to commit."
fi

# Try to sync with remote before pushing
set +e
git fetch "$remote_name" "$branch" 2>/dev/null
upstream_set=false
if git rev-parse --abbrev-ref --symbolic-full-name @{u} >/dev/null 2>&1; then
  upstream_set=true
fi

if [[ "$upstream_set" == true ]]; then
  git pull --rebase "$remote_name" "$branch" || true
fi
set -e

# Push with upstream if not set
if [[ "$upstream_set" == true ]]; then
  git push "$remote_name" "$branch"
else
  git push -u "$remote_name" "$branch"
fi

echo "Pushed to $remote_name/$branch successfully."


