FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Group.fsproj", "./"]
RUN dotnet restore "Group.fsproj"
COPY . .
WORKDIR /src/src
RUN dotnet build "Group.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Group.fsproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Group.dll"]
