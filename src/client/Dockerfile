FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY [ "src/." , "./" ]
COPY [ "nginx/nginx.conf" , "./" ]

RUN dotnet restore "Client.csproj"
RUN dotnet build "Client.csproj" -c Release -o /build

FROM build-env AS publish
RUN dotnet publish "Client.csproj" -c Release -o /publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

COPY --from=publish [ "/publish/wwwroot" , "/usr/local/webapp/nginx/html" ]
COPY [ "nginx.conf" , "/etc/nginx/nginx.conf" ]