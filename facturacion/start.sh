#!/bin/bash
# Render optimized start script
cd Backend
echo "Starting .NET application on port $PORT"
dotnet run --urls http://0.0.0.0:$PORT --environment Production