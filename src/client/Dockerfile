### BUILD ###
FROM node:14 AS build

WORKDIR /usr/src/app

COPY src/package.json src/yarn.lock ./

RUN npm i -g @angular/cli
RUN npm i --only=prod

COPY [ "src/." , "./" ]

RUN ng build --configuration production

### RUN ###
FROM nginx:alpine
EXPOSE 80
COPY --from=build [ "/usr/src/app/dist/prometheus/." , "/usr/local/webapp/nginx/html" ]
COPY              [ "nginx/nginx.conf"               , "/etc/nginx" ]
RUN ls -la /usr/local/webapp/nginx/html