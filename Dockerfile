# syntax=docker/dockerfile:1.7-labs
#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
LABEL org.opencontainers.image.source=https://github.com/Gin-sen/Ginsen.Net8.Async.Template
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
USER app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore
WORKDIR /src
COPY ["Ginsen.Net8.Async.Milestone.Api/Ginsen.Net8.Async.Milestone.Api.csproj", "Ginsen.Net8.Async.Milestone.Api/"]
COPY ["Ginsen.Net8.Async.Milestone.Api.Contracts/Ginsen.Net8.Async.Milestone.Api.Contracts.csproj", "Ginsen.Net8.Async.Milestone.Api.Contracts/"]
COPY ["Ginsen.Net8.Async.Milestone.Application/Ginsen.Net8.Async.Milestone.Application.csproj", "Ginsen.Net8.Async.Milestone.Application/"]
COPY ["Ginsen.Net8.Async.Milestone.BlazorBackOffice/Ginsen.Net8.Async.Milestone.BlazorBackOffice.csproj", "Ginsen.Net8.Async.Milestone.BlazorBackOffice/"]
COPY ["Ginsen.Net8.Async.Milestone.BlazorBackOffice.Client/Ginsen.Net8.Async.Milestone.BlazorBackOffice.Client.csproj", "Ginsen.Net8.Async.Milestone.BlazorBackOffice.Client/"]
COPY ["Ginsen.Net8.Async.Milestone.Domain/Ginsen.Net8.Async.Milestone.Domain.csproj", "Ginsen.Net8.Async.Milestone.Domain/"]
COPY ["Ginsen.Net8.Async.Milestone.HttpClientGenerator/Ginsen.Net8.Async.Milestone.HttpClientGenerator.csproj", "Ginsen.Net8.Async.Milestone.HttpClientGenerator/"]
COPY ["Ginsen.Net8.Async.Milestone.Contracts.Messaging/Ginsen.Net8.Async.Milestone.Contracts.Messaging.csproj", "Ginsen.Net8.Async.Milestone.Contracts.Messaging/"]
COPY ["Ginsen.Net8.Async.Milestone.Infrastructure.AzureStorageAccount/Ginsen.Net8.Async.Milestone.Infrastructure.AzureStorageAccount.csproj", "Ginsen.Net8.Async.Milestone.Infrastructure.AzureStorageAccount/"]
COPY ["Ginsen.Net8.Async.Milestone.Worker/Ginsen.Net8.Async.Milestone.Worker.csproj", "Ginsen.Net8.Async.Milestone.Worker/"]
RUN dotnet restore "./Ginsen.Net8.Async.Milestone.Api/Ginsen.Net8.Async.Milestone.Api.csproj" && \
  dotnet restore "./Ginsen.Net8.Async.Milestone.Worker/Ginsen.Net8.Async.Milestone.Worker.csproj" && \
  dotnet restore "./Ginsen.Net8.Async.Milestone.BlazorBackOffice/Ginsen.Net8.Async.Milestone.BlazorBackOffice.csproj" && \
  dotnet restore "./Ginsen.Net8.Async.Milestone.BlazorBackOffice.Client/Ginsen.Net8.Async.Milestone.BlazorBackOffice.Client.csproj"
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

FROM restore AS build-backoffice
ARG BUILD_CONFIGURATION=Release
COPY ["Ginsen.Net8.Async.Milestone.BlazorBackOffice/appsettings*.json", "Ginsen.Net8.Async.Milestone.BlazorBackOffice/"]
WORKDIR "/src/Ginsen.Net8.Async.Milestone.BlazorBackOffice"
RUN dotnet build "./Ginsen.Net8.Async.Milestone.BlazorBackOffice.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build-backoffice AS publish-backoffice
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Ginsen.Net8.Async.Milestone.BlazorBackOffice.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final-backoffice
WORKDIR /app
COPY --from=publish-backoffice /app/publish .
ENTRYPOINT ["dotnet", "Ginsen.Net8.Async.Milestone.BlazorBackOffice.dll"]

FROM restore AS build-backoffice-client
ARG BUILD_CONFIGURATION=Release
COPY ["Ginsen.Net8.Async.Milestone.BlazorBackOffice.Client/wwwroot/appsettings*.json", "Ginsen.Net8.Async.Milestone.BlazorBackOffice.Client/wwwroot"]
WORKDIR "/src/Ginsen.Net8.Async.Milestone.BlazorBackOffice.Client"
RUN dotnet build "./Ginsen.Net8.Async.Milestone.BlazorBackOffice.Client.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build-backoffice-client AS publish-backoffice-client
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Ginsen.Net8.Async.Milestone.BlazorBackOffice.Client.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM nginx:alpine AS final-backoffice-client
WORKDIR /usr/share/nginx/html
COPY --from=publish-backoffice-client /app/publish/wwwroot .
COPY Ginsen.Net8.Async.Milestone.BlazorBackOffice.Client/nginx.conf /etc/nginx/nginx.conf

HEALTHCHECK --interval=1m --timeout=3s \
  CMD curl -f http://localhost:8080/health || exit 1
