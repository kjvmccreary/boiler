# docker/services/Frontend.Dockerfile
# Stage 1: Build
FROM node:20-alpine AS build
WORKDIR /app

# Copy package files for better caching
COPY src/frontend/react-app/package*.json ./

# Install dependencies
RUN npm ci --silent

# Copy source code
COPY src/frontend/react-app/ .

# Build the application
RUN npm run build

# Stage 2: Production (Nginx with SSL)
FROM nginx:alpine AS final

# Install curl for health checks
RUN apk add --no-cache curl

# Create SSL directory
RUN mkdir -p /etc/nginx/ssl

# Copy custom nginx config
COPY docker/nginx/default.conf /etc/nginx/conf.d/default.conf

# Copy SSL certificates
COPY docker/nginx/ssl/localhost.crt /etc/nginx/ssl/localhost.crt
COPY docker/nginx/ssl/localhost.key /etc/nginx/ssl/localhost.key

# Copy built application from build stage
COPY --from=build /app/dist /usr/share/nginx/html
# Set permissions for SSL certificates
# Health check endpoint (use HTTPS)
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f -k https://localhost:443/health || exit 1

EXPOSE 80 443
CMD ["nginx", "-g", "daemon off;"]
