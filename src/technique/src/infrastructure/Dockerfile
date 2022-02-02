FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/infrastructure/Technique.Infrastructure.fsproj", "infrastructure/"]
COPY ["src/application/Technique.Application.fsproj", "application/"]
COPY ["src/domain/Technique.Domain.fsproj", "domain/"]
RUN dotnet restore "src/infrastructure/Technique.Infrastructure.fsproj"
COPY . .
WORKDIR "/src/infrastructure"
RUN dotnet build "Technique.Infrastructure.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Technique.Infrastructure.fsproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Technique.Infrastructure.dll"]
