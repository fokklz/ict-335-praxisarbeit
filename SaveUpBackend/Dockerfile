#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Base stage for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
EXPOSE 8000

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SaveUpBackend/SaveUpBackend.csproj", "SaveUpBackend/"]
RUN dotnet restore "./SaveUpBackend/SaveUpBackend.csproj"
COPY . .
WORKDIR "/src/SaveUpBackend"
RUN dotnet build "./SaveUpBackend.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SaveUpBackend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER root

# Install SurrealDB
RUN apt-get update && apt-get install -y curl
RUN curl -sSf https://install.surrealdb.com | sh

# Script to run both SurrealDB and the .NET application
COPY SaveUpBackend/entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

USER app

ENTRYPOINT ["/entrypoint.sh"]