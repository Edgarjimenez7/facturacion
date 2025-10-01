#!/bin/bash
# Build script for Render
echo "Current directory: $(pwd)"
echo "Listing files:"
ls -la
echo "Looking for Backend directory..."
find . -name "Backend" -type d
echo "Building application..."
cd facturacion/Backend
dotnet restore
dotnet build -c Release