FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Technique.fsproj", "./"]
RUN dotnet restore "Technique.fsproj"
COPY . .
WORKDIR /src/src
RUN dotnet build "Technique.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Technique.fsproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Technique.dll"]
