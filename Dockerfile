# syntax=docker/dockerfile:1.7-labs
#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
LABEL org.opencontainers.image.source=https://github.com/Gin-sen/Ginsen.Net8.Async.Template
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore
WORKDIR /src
COPY ["Ginsen.Net8.Async.Milestone.Api/Ginsen.Net8.Async.Milestone.Api.csproj", "Ginsen.Net8.Async.Milestone.Api/"]
COPY ["Ginsen.Net8.Async.Milestone.Contracts.Http/Ginsen.Net8.Async.Milestone.Contracts.Http.csproj", "Ginsen.Net8.Async.Milestone.Contracts.Http/"]
COPY ["Ginsen.Net8.Async.Milestone.Contracts.Messaging/Ginsen.Net8.Async.Milestone.Contracts.Messaging.csproj", "Ginsen.Net8.Async.Milestone.Contracts.Messaging/"]
COPY ["Ginsen.Net8.Async.Milestone.Infrastructure/Ginsen.Net8.Async.Milestone.Infrastructure.csproj", "Ginsen.Net8.Async.Milestone.Infrastructure/"]
COPY ["Ginsen.Net8.Async.Milestone.Worker/Ginsen.Net8.Async.Milestone.Worker.csproj", "Ginsen.Net8.Async.Milestone.Worker/"]
RUN dotnet restore "./Ginsen.Net8.Async.Milestone.Api/Ginsen.Net8.Async.Milestone.Api.csproj" && \
dotnet restore "./Ginsen.Net8.Async.Milestone.Worker/Ginsen.Net8.Async.Milestone.Worker.csproj"
COPY --exclude=appsettings*.json . .

FROM restore AS build-worker
ARG BUILD_CONFIGURATION=Release
COPY ["Ginsen.Net8.Async.Milestone.Worker/appsettings*.json", "Ginsen.Net8.Async.Milestone.Worker/"]
WORKDIR "/src/Ginsen.Net8.Async.Milestone.Worker"
RUN dotnet build "./Ginsen.Net8.Async.Milestone.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build-worker AS publish-worker
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Ginsen.Net8.Async.Milestone.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final-worker
WORKDIR /app
COPY --from=publish-worker /app/publish .
ENTRYPOINT ["dotnet", "Ginsen.Net8.Async.Milestone.Worker.dll"]

FROM restore AS build-api
ARG BUILD_CONFIGURATION=Release
COPY ["Ginsen.Net8.Async.Milestone.Api/appsettings*.json", "Ginsen.Net8.Async.Milestone.Api/"]
WORKDIR "/src/Ginsen.Net8.Async.Milestone.Api"
RUN dotnet build "./Ginsen.Net8.Async.Milestone.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build-api AS publish-api
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Ginsen.Net8.Async.Milestone.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final-api
WORKDIR /app
COPY --from=publish-api /app/publish .
ENTRYPOINT ["dotnet", "Ginsen.Net8.Async.Milestone.Api.dll"]

HEALTHCHECK --interval=1m --timeout=3s \
  CMD curl -f http://localhost:8080/health || exit 1
