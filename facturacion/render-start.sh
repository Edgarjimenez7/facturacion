#!/bin/bash
# Start script for Render
echo "Starting application..."
export PATH="$HOME/.dotnet:$PATH"
export DOTNET_ROOT="$HOME/.dotnet"
cd Backend
dotnet run --urls http://0.0.0.0:$PORT --environment Production