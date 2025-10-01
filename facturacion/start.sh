#!/bin/bash
# Render start script
cd Backend
dotnet run --urls http://0.0.0.0:$PORT -c Release