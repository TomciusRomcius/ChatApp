FROM node:21-alpine AS base
WORKDIR /app
COPY package*.json ./
RUN npm i
COPY . .
EXPOSE 3000
# Must build in AWS because NEXT_PUBLIC environment variables are injected at build-time
CMD ["sh", "-c", "npm run build && npm run start"]

FROM base AS development
WORKDIR /app
CMD [ "sh", "-c", "npm", "run", "dev:https" ]
