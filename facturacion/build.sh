#!/bin/bash
# Render build script
cd Backend
dotnet restore
dotnet build -c Release