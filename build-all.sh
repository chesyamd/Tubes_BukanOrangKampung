#!/bin/sh
set -e

echo "=== Build main bot ==="
find src/main-bot -name "*.csproj" | while read project; do
  echo "Building $project"
  dotnet build "$project"
done

echo "=== Build alternative bots ==="
find src/alternative-bots -name "*.csproj" | while read project; do
  echo "Building $project"
  dotnet build "$project"
done

echo "All bots built successfully."
