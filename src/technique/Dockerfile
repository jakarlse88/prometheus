﻿# Base
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY [ "src/infrastructure/", "infrastructure/" ]
COPY [ "src/application/", "application/" ]
COPY [ "src/domain/", "domain/" ]

RUN dotnet restore "infrastructure/Technique.Infrastructure.fsproj"

COPY . .
RUN ls -la /

WORKDIR "/src/infrastructure"

RUN dotnet build "Technique.Infrastructure.fsproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "Technique.Infrastructure.fsproj" -c Release -o /app/publish

# Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Technique.Infrastructure.dll"]
