# Stage 1: Build the Angular SPA application
FROM node:22-alpine AS build
WORKDIR /app

# Copy package files and install dependencies
COPY package*.json ./
RUN npm ci

# Copy source files and build production bundle
COPY . .
RUN npm run build -- --configuration production

# Stage 2: Serve application using lightweight Nginx Alpine
FROM nginx:alpine AS runtime
COPY --from=build /app/dist/erp-admin/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
