FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Analysis.fsproj", "./"]
RUN dotnet restore "Analysis.fsproj"
COPY . .
WORKDIR /src/src
RUN dotnet build "Analysis.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Analysis.fsproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Analysis.dll"]
