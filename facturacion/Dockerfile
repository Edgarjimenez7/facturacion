# Multi-stage Docker build for .NET 6 Application
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE $PORT
ENV ASPNETCORE_URLS=http://*:$PORT
ENV ASPNETCORE_ENVIRONMENT=Production

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Backend/FacturacionAPI.csproj", "Backend/"]
RUN dotnet restore "Backend/FacturacionAPI.csproj"
COPY . .
WORKDIR "/src/Backend"
RUN dotnet build "FacturacionAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FacturacionAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Copy static frontend files
COPY Frontend ./wwwroot
ENTRYPOINT ["dotnet", "FacturacionAPI.dll"]