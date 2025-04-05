FROM node:21-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm i
COPY . .
# temporary no lint
RUN npm run build --no-lint

FROM node:21-alpine AS final
WORKDIR /app
COPY --from=build /app/.next .
ENTRYPOINT [ "node", "./standalone/server.js", "start" ]