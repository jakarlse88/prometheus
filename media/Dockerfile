FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Media.fsproj", "./"]
RUN dotnet restore "Media.fsproj"
COPY . .
WORKDIR /src/src
RUN dotnet build "Media.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Media.fsproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Media.dll"]
