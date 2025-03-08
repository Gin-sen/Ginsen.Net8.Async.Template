#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
LABEL org.opencontainers.image.source=https://github.com/Gin-sen/Ginsen.Net8.Async.Template
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Ginsen.Net8.Async.Milestone.Api/Ginsen.Net8.Async.Milestone.Api.csproj", "Ginsen.Net8.Async.Milestone.Api/"]
RUN dotnet restore "./Ginsen.Net8.Async.Milestone.Api/Ginsen.Net8.Async.Milestone.Api.csproj"
COPY . .
WORKDIR "/src/Ginsen.Net8.Async.Milestone.Api"
RUN dotnet build "./Ginsen.Net8.Async.Milestone.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Ginsen.Net8.Async.Milestone.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ginsen.Net8.Async.Milestone.Api.dll"]

HEALTHCHECK --interval=1m --timeout=3s \
  CMD curl -f http://localhost:8080/health || exit 1