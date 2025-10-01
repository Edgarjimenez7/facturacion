#!/bin/bash
# Build script for Render with .NET installation
echo "Current directory: $(pwd)"
echo "Installing .NET 6 SDK..."
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version 6.0.425
export PATH="$HOME/.dotnet:$PATH"
export DOTNET_ROOT="$HOME/.dotnet"
echo "Verifying .NET installation..."
dotnet --version
echo "Building application..."
cd Backend
dotnet restore
dotnet build -c Release