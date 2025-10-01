#!/bin/bash
# Start script for Render
echo "Starting application..."
cd facturacion/Backend
dotnet run --urls http://0.0.0.0:$PORT --environment Production