FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Gateway.fsproj", "./"]
RUN dotnet restore "Gateway.fsproj"
COPY . .
WORKDIR /src/src
RUN dotnet build "Gateway.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Gateway.fsproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gateway.dll"]
