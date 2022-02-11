
#stage 1
FROM node:latest as build-step
RUN mkdir -p /app
WORKDIR /app
COPY package.json /app

RUN npm install
COPY . /app
RUN npm run build --prod
#stage 2
FROM nginx:latest
COPY nginx.conf /etc/nginx/nginx.conf
COPY --from=build-step /app/dist/stock-chart /usr/share/nginx/html