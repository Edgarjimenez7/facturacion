#!/bin/bash
# Start script for Render with .NET verification
echo "Starting application..."
export PATH="$HOME/.dotnet:$PATH"
export DOTNET_ROOT="$HOME/.dotnet"
echo "Checking .NET installation..."
if [ ! -f "$HOME/.dotnet/dotnet" ]; then
    echo "Installing .NET SDK..."
    curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version 6.0.425
fi
echo "Using .NET version: $(dotnet --version)"
cd Backend
dotnet run --urls http://0.0.0.0:$PORT --environment Production