# 📦 Phase 8: Docker Configuration - Detailed Implementation Plan

## 📊 Phase Overview
- **Duration**: 2-3 sessions (6-12 hours)
- **Complexity**: Medium
- **Prerequisites**: ✅ Phases 1-7 complete, Docker Desktop installed

## 🎯 Phase 8 Objectives
1. Containerize all services (AuthService, UserService, ApiGateway, React Frontend)
2. Create optimized multi-stage builds
3. Set up Docker Compose for local development
4. Configure service networking and communication
5. Implement database initialization and migrations
6. Add nginx reverse proxy for production-like setup

---

## 📁 Project Structure to Create

```
boiler/
├── docker/                              # All Docker-related files
│   ├── services/                       # Service-specific Dockerfiles
│   │   ├── AuthService.Dockerfile
│   │   ├── UserService.Dockerfile
│   │   ├── ApiGateway.Dockerfile
│   │   └── Frontend.Dockerfile
│   ├── nginx/                          # Nginx configuration
│   │   ├── nginx.conf
│   │   └── default.conf
│   ├── postgres/                       # Database initialization
│   │   └── init.sql
│   ├── docker-compose.yml              # Production-like setup
│   ├── docker-compose.override.yml     # Development overrides
│   └── docker-compose.infrastructure.yml # Infrastructure only
├── scripts/                            # Helper scripts
│   ├── start-dev.ps1                  # Windows
│   ├── start-dev.sh                   # Linux/Mac
│   ├── stop-dev.ps1
│   ├── stop-dev.sh
│   ├── rebuild.ps1
│   └── rebuild.sh
└── .env.docker                         # Docker environment variables
```

---

## 🔨 Deliverable 1: Multi-Stage Dockerfiles

### 1.1 AuthService.Dockerfile
```dockerfile
# docker/services/AuthService.Dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["src/services/AuthService/AuthService.csproj", "services/AuthService/"]
COPY ["src/shared/DTOs/DTOs.csproj", "shared/DTOs/"]
COPY ["src/shared/Common/Common.csproj", "shared/Common/"]
COPY ["src/shared/Contracts/Contracts.csproj", "shared/Contracts/"]

# Restore dependencies
RUN dotnet restore "services/AuthService/AuthService.csproj"

# Copy source code
COPY src/services/AuthService/ services/AuthService/
COPY src/shared/ shared/

# Build application
WORKDIR "/src/services/AuthService"
RUN dotnet build "AuthService.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "AuthService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

# Switch to non-root user
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5001/health || exit 1

EXPOSE 5001
ENTRYPOINT ["dotnet", "AuthService.dll"]
```

### 1.2 UserService.Dockerfile
```dockerfile
# docker/services/UserService.Dockerfile
# Similar structure to AuthService
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/services/UserService/UserService.csproj", "services/UserService/"]
COPY ["src/shared/DTOs/DTOs.csproj", "shared/DTOs/"]
COPY ["src/shared/Common/Common.csproj", "shared/Common/"]
COPY ["src/shared/Contracts/Contracts.csproj", "shared/Contracts/"]

RUN dotnet restore "services/UserService/UserService.csproj"

COPY src/services/UserService/ services/UserService/
COPY src/shared/ shared/

WORKDIR "/src/services/UserService"
RUN dotnet build "UserService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN groupadd -r appuser && useradd -r -g appuser appuser
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .
USER appuser

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5002/health || exit 1

EXPOSE 5002
ENTRYPOINT ["dotnet", "UserService.dll"]
```

### 1.3 ApiGateway.Dockerfile
```dockerfile
# docker/services/ApiGateway.Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/services/ApiGateway/ApiGateway.csproj", "services/ApiGateway/"]
COPY ["src/shared/DTOs/DTOs.csproj", "shared/DTOs/"]
COPY ["src/shared/Common/Common.csproj", "shared/Common/"]

RUN dotnet restore "services/ApiGateway/ApiGateway.csproj"

COPY src/services/ApiGateway/ services/ApiGateway/
COPY src/shared/ shared/

WORKDIR "/src/services/ApiGateway"
RUN dotnet build "ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN groupadd -r appuser && useradd -r -g appuser appuser
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .
USER appuser

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

EXPOSE 5000
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
```

### 1.4 Frontend.Dockerfile
```dockerfile
# docker/services/Frontend.Dockerfile
# Stage 1: Build
FROM node:20-alpine AS build
WORKDIR /app

# Copy package files
COPY src/frontend/react-app/package*.json ./

# Install dependencies
RUN npm ci --silent

# Copy source code
COPY src/frontend/react-app/ .

# Build application
RUN npm run build

# Stage 2: Production
FROM nginx:alpine AS final

# Install curl for health checks
RUN apk add --no-cache curl

# Copy custom nginx config
COPY docker/nginx/default.conf /etc/nginx/conf.d/default.conf

# Copy built application
COPY --from=build /app/dist /usr/share/nginx/html

# Create non-root user
RUN addgroup -g 101 -S nginx && \
    adduser -S -D -H -u 101 -h /var/cache/nginx -s /sbin/nologin -G nginx -g nginx nginx

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:80/health || exit 1

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

---

## 🔨 Deliverable 2: Docker Compose Configuration

### 2.1 docker-compose.yml (Production-like)
```yaml
# docker/docker-compose.yml
version: '3.8'

services:
  # Database
  postgres:
    image: postgres:15-alpine
    container_name: boiler-postgres
    environment:
      POSTGRES_DB: ${POSTGRES_DB:-boilerdb}
      POSTGRES_USER: ${POSTGRES_USER:-boileruser}
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD:-boilerpass}
    volumes:
      - postgres_data:/var/lib/postgresql/data
      - ./postgres/init.sql:/docker-entrypoint-initdb.d/init.sql
    ports:
      - "5432:5432"
    networks:
      - boiler-network
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U ${POSTGRES_USER:-boileruser}"]
      interval: 10s
      timeout: 5s
      retries: 5

  # Cache
  redis:
    image: redis:7-alpine
    container_name: boiler-redis
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data
    networks:
      - boiler-network
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 10s
      timeout: 5s
      retries: 5

  # Auth Service
  auth-service:
    build:
      context: ..
      dockerfile: docker/services/AuthService.Dockerfile
    container_name: boiler-auth
    environment:
      ASPNETCORE_ENVIRONMENT: Docker
      ConnectionStrings__DefaultConnection: "Host=postgres;Database=${POSTGRES_DB:-boilerdb};Username=${POSTGRES_USER:-boileruser};Password=${POSTGRES_PASSWORD:-boilerpass}"
      JwtSettings__SecretKey: ${JWT_SECRET_KEY}
      Redis__ConnectionString: "redis:6379"
    depends_on:
      postgres:
        condition: service_healthy
      redis:
        condition: service_healthy
    ports:
      - "5001:5001"
    networks:
      - boiler-network

  # User Service
  user-service:
    build:
      context: ..
      dockerfile: docker/services/UserService.Dockerfile
    container_name: boiler-user
    environment:
      ASPNETCORE_ENVIRONMENT: Docker
      ConnectionStrings__DefaultConnection: "Host=postgres;Database=${POSTGRES_DB:-boilerdb};Username=${POSTGRES_USER:-boileruser};Password=${POSTGRES_PASSWORD:-boilerpass}"
      JwtSettings__SecretKey: ${JWT_SECRET_KEY}
    depends_on:
      postgres:
        condition: service_healthy
    ports:
      - "5002:5002"
    networks:
      - boiler-network

  # API Gateway
  api-gateway:
    build:
      context: ..
      dockerfile: docker/services/ApiGateway.Dockerfile
    container_name: boiler-gateway
    environment:
      ASPNETCORE_ENVIRONMENT: Docker
      Services__AuthService: "http://auth-service:5001"
      Services__UserService: "http://user-service:5002"
      JwtSettings__SecretKey: ${JWT_SECRET_KEY}
    depends_on:
      - auth-service
      - user-service
    ports:
      - "5000:5000"
    networks:
      - boiler-network

  # Frontend
  frontend:
    build:
      context: ..
      dockerfile: docker/services/Frontend.Dockerfile
      args:
        VITE_API_URL: http://localhost:5000/api
    container_name: boiler-frontend
    ports:
      - "3000:80"
    depends_on:
      - api-gateway
    networks:
      - boiler-network

  # Reverse Proxy
  nginx:
    image: nginx:alpine
    container_name: boiler-nginx
    volumes:
      - ./nginx/nginx.conf:/etc/nginx/nginx.conf:ro
    ports:
      - "80:80"
      - "443:443"
    depends_on:
      - frontend
      - api-gateway
    networks:
      - boiler-network

volumes:
  postgres_data:
  redis_data:

networks:
  boiler-network:
    driver: bridge
```

### 2.2 docker-compose.override.yml (Development)
```yaml
# docker/docker-compose.override.yml
version: '3.8'

services:
  postgres:
    ports:
      - "5432:5432"
    environment:
      POSTGRES_PASSWORD: devpassword123

  auth-service:
    build:
      target: build  # Use build stage for hot reload
    volumes:
      - ../src/services/AuthService:/src/services/AuthService
      - ../src/shared:/src/shared
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      DOTNET_WATCH: 1
    command: ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:5001"]

  user-service:
    build:
      target: build
    volumes:
      - ../src/services/UserService:/src/services/UserService
      - ../src/shared:/src/shared
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      DOTNET_WATCH: 1
    command: ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:5002"]

  api-gateway:
    build:
      target: build
    volumes:
      - ../src/services/ApiGateway:/src/services/ApiGateway
      - ../src/shared:/src/shared
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      DOTNET_WATCH: 1
    command: ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:5000"]

  frontend:
    build:
      target: build
    volumes:
      - ../src/frontend/react-app:/app
      - /app/node_modules  # Prevent overwriting node_modules
    environment:
      NODE_ENV: development
    command: ["npm", "run", "dev"]
    ports:
      - "5173:5173"  # Vite dev server port

  # Development tools
  pgadmin:
    image: dpage/pgadmin4
    container_name: boiler-pgadmin
    environment:
      PGADMIN_DEFAULT_EMAIL: admin@boiler.com
      PGADMIN_DEFAULT_PASSWORD: admin123
    ports:
      - "8080:80"
    networks:
      - boiler-network
```

### 2.3 docker-compose.infrastructure.yml
```yaml
# docker/docker-compose.infrastructure.yml
# Infrastructure only - for when you want to run services locally but use Docker for DB/Redis
version: '3.8'

services:
  postgres:
    image: postgres:15-alpine
    container_name: boiler-postgres-dev
    environment:
      POSTGRES_DB: boilerdb_dev
      POSTGRES_USER: boileruser
      POSTGRES_PASSWORD: localdev123
    ports:
      - "5432:5432"
    volumes:
      - postgres_dev_data:/var/lib/postgresql/data

  redis:
    image: redis:7-alpine
    container_name: boiler-redis-dev
    ports:
      - "6379:6379"
    volumes:
      - redis_dev_data:/data

volumes:
  postgres_dev_data:
  redis_dev_data:
```

---

## 🔨 Deliverable 3: Nginx Configuration

### 3.1 nginx.conf
```nginx
# docker/nginx/nginx.conf
events {
    worker_connections 1024;
}

http {
    upstream api_gateway {
        server api-gateway:5000;
    }

    upstream frontend {
        server frontend:80;
    }

    # Rate limiting
    limit_req_zone $binary_remote_addr zone=api_limit:10m rate=10r/s;
    limit_req_zone $binary_remote_addr zone=auth_limit:10m rate=5r/s;

    # Main server block
    server {
        listen 80;
        server_name localhost;

        # Security headers
        add_header X-Frame-Options "SAMEORIGIN" always;
        add_header X-Content-Type-Options "nosniff" always;
        add_header X-XSS-Protection "1; mode=block" always;

        # Frontend
        location / {
            proxy_pass http://frontend;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto $scheme;
        }

        # API Gateway
        location /api {
            limit_req zone=api_limit burst=20 nodelay;
            
            proxy_pass http://api_gateway;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto $scheme;
            
            # CORS headers
            add_header 'Access-Control-Allow-Origin' '*' always;
            add_header 'Access-Control-Allow-Methods' 'GET, POST, PUT, DELETE, OPTIONS' always;
            add_header 'Access-Control-Allow-Headers' 'DNT,User-Agent,X-Requested-With,If-Modified-Since,Cache-Control,Content-Type,Range,Authorization' always;
        }

        # Auth endpoints with stricter rate limiting
        location /api/auth {
            limit_req zone=auth_limit burst=5 nodelay;
            
            proxy_pass http://api_gateway;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
        }

        # Health checks
        location /health {
            access_log off;
            return 200 "healthy\n";
        }
    }
}
```

### 3.2 default.conf (Frontend nginx)
```nginx
# docker/nginx/default.conf
server {
    listen 80;
    server_name localhost;
    root /usr/share/nginx/html;
    index index.html;

    # Gzip
    gzip on;
    gzip_vary on;
    gzip_min_length 1024;
    gzip_types text/plain text/css text/xml application/json application/javascript application/xml+rss application/atom+xml image/svg+xml;

    # Cache static assets
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }

    # React app - serve index.html for all routes
    location / {
        try_files $uri $uri/ /index.html;
    }

    # Security headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
}
```

---

## 🔨 Deliverable 4: Database Initialization

### 4.1 init.sql
```sql
-- docker/postgres/init.sql
-- Create database if not exists
CREATE DATABASE boilerdb;

-- Connect to the database
\c boilerdb;

-- Create extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Note: EF Core migrations will handle table creation
-- This file is for any initial setup or seed data

-- Create default tenant
INSERT INTO "Tenants" ("Id", "Name", "Subdomain", "IsActive", "CreatedAt", "UpdatedAt")
VALUES 
  (1, 'Default Tenant', 'default', true, NOW(), NOW())
ON CONFLICT ("Id") DO NOTHING;

-- Create default permissions (if not using EF seeding)
-- These would normally be created by your migration/seed process
```

---

## 🔨 Deliverable 5: Helper Scripts

### 5.1 start-dev.ps1 (Windows PowerShell)
```powershell
# scripts/start-dev.ps1
Write-Host "Starting Boiler Development Environment..." -ForegroundColor Green

# Check if Docker is running
$dockerStatus = docker info 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "Docker is not running. Please start Docker Desktop." -ForegroundColor Red
    exit 1
}

# Load environment variables
if (Test-Path .env.docker) {
    Get-Content .env.docker | ForEach-Object {
        if ($_ -match '^([^#].*)=(.*)$') {
            [System.Environment]::SetEnvironmentVariable($matches[1], $matches[2])
        }
    }
}

# Build and start services
Write-Host "Building services..." -ForegroundColor Yellow
docker-compose -f docker/docker-compose.yml -f docker/docker-compose.override.yml build

Write-Host "Starting services..." -ForegroundColor Yellow
docker-compose -f docker/docker-compose.yml -f docker/docker-compose.override.yml up -d

# Wait for services to be healthy
Write-Host "Waiting for services to be healthy..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Run migrations
Write-Host "Running database migrations..." -ForegroundColor Yellow
docker exec boiler-auth dotnet ef database update
docker exec boiler-user dotnet ef database update

Write-Host "Development environment started successfully!" -ForegroundColor Green
Write-Host "Services available at:" -ForegroundColor Cyan
Write-Host "  Frontend: http://localhost:3000" -ForegroundColor White
Write-Host "  API Gateway: http://localhost:5000" -ForegroundColor White
Write-Host "  PgAdmin: http://localhost:8080" -ForegroundColor White
```

### 5.2 start-dev.sh (Linux/Mac)
```bash
#!/bin/bash
# scripts/start-dev.sh

echo -e "\033[32mStarting Boiler Development Environment...\033[0m"

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo -e "\033[31mDocker is not running. Please start Docker.\033[0m"
    exit 1
fi

# Load environment variables
if [ -f .env.docker ]; then
    export $(cat .env.docker | grep -v '^#' | xargs)
fi

# Build and start services
echo -e "\033[33mBuilding services...\033[0m"
docker-compose -f docker/docker-compose.yml -f docker/docker-compose.override.yml build

echo -e "\033[33mStarting services...\033[0m"
docker-compose -f docker/docker-compose.yml -f docker/docker-compose.override.yml up -d

# Wait for services
echo -e "\033[33mWaiting for services to be healthy...\033[0m"
sleep 10

# Run migrations
echo -e "\033[33mRunning database migrations...\033[0m"
docker exec boiler-auth dotnet ef database update
docker exec boiler-user dotnet ef database update

echo -e "\033[32mDevelopment environment started successfully!\033[0m"
echo -e "\033[36mServices available at:\033[0m"
echo "  Frontend: http://localhost:3000"
echo "  API Gateway: http://localhost:5000"
echo "  PgAdmin: http://localhost:8080"
```

### 5.3 stop-dev.ps1 (Windows)
```powershell
# scripts/stop-dev.ps1
Write-Host "Stopping Boiler Development Environment..." -ForegroundColor Yellow

docker-compose -f docker/docker-compose.yml -f docker/docker-compose.override.yml down

Write-Host "Development environment stopped." -ForegroundColor Green
```

### 5.4 rebuild.sh (Linux/Mac)
```bash
#!/bin/bash
# scripts/rebuild.sh

echo -e "\033[33mRebuilding Boiler services...\033[0m"

# Stop existing containers
docker-compose -f docker/docker-compose.yml -f docker/docker-compose.override.yml down

# Rebuild without cache
docker-compose -f docker/docker-compose.yml -f docker/docker-compose.override.yml build --no-cache

# Start services
docker-compose -f docker/docker-compose.yml -f docker/docker-compose.override.yml up -d

echo -e "\033[32mRebuild complete!\033[0m"
```

### 5.5 Environment Variables (.env.docker)
```env
# .env.docker
# PostgreSQL
POSTGRES_DB=boilerdb
POSTGRES_USER=boileruser
POSTGRES_PASSWORD=boilerpass123!

# JWT
JWT_SECRET_KEY=your-super-secret-jwt-key-that-is-at-least-256-bits-long-for-security

# Redis
REDIS_CONNECTION=redis:6379

# Application
ASPNETCORE_ENVIRONMENT=Docker
```

---

## 📋 Implementation Checklist

### Session 1: Basic Containerization (3-4 hours)
- [ ] Create docker directory structure
- [ ] Write Dockerfile for AuthService
- [ ] Write Dockerfile for UserService
- [ ] Write Dockerfile for ApiGateway
- [ ] Write Dockerfile for Frontend
- [ ] Test individual container builds

### Session 2: Docker Compose Setup (3-4 hours)
- [ ] Create docker-compose.yml
- [ ] Create docker-compose.override.yml
- [ ] Create docker-compose.infrastructure.yml
- [ ] Configure service networking
- [ ] Set up environment variables
- [ ] Test full stack startup

### Session 3: Production Optimizations (3-4 hours)
- [ ] Configure nginx reverse proxy
- [ ] Add health checks to all services
- [ ] Implement database initialization
- [ ] Create helper scripts
- [ ] Add monitoring/logging setup
- [ ] Document deployment process

---

## 🧪 Testing Phase 8

### 1. Individual Container Tests
```bash
# Build individual services
docker build -f docker/services/AuthService.Dockerfile -t boiler-auth .
docker build -f docker/services/UserService.Dockerfile -t boiler-user .

# Run individual containers
docker run -p 5001:5001 boiler-auth
docker run -p 5002:5002 boiler-user
```

### 2. Integration Tests
```bash
# Start infrastructure only
docker-compose -f docker/docker-compose.infrastructure.yml up -d

# Start full stack
docker-compose -f docker/docker-compose.yml up -d

# Check health
curl http://localhost:5000/health
curl http://localhost:5001/health
curl http://localhost:5002/health
```

### 3. Performance Tests
```bash
# Check image sizes
docker images | grep boiler

# Check memory usage
docker stats

# Check logs
docker-compose logs -f auth-service
```

---

## 🎯 Success Criteria

✅ **Phase 8 is complete when:**
1. All services build and run in Docker containers
2. Services can communicate through Docker network
3. Database migrations run automatically
4. Frontend can call API through gateway
5. Development workflow is smooth (hot reload works)
6. Production build is optimized (<100MB per service)
7. Health checks pass for all services
8. Helper scripts work on Windows/Mac/Linux

---

## 📚 Next Steps (Phase 9 Preview)

After Phase 8, you'll enhance multi-tenancy:
- Implement tenant resolution strategies
- Add Row-Level Security in PostgreSQL
- Create tenant management UI
- Test tenant isolation thoroughly

---

## 💡 Pro Tips

1. **Use BuildKit** for faster builds:
   ```bash
   export DOCKER_BUILDKIT=1
   ```

2. **Use .dockerignore** to reduce context size:
   ```
   **/bin
   **/obj
   **/node_modules
   **/.git
   ```

3. **Monitor resource usage** during development:
   ```bash
   docker system df
   docker system prune -a
   ```

4. **Debug containers** when needed:
   ```bash
   docker exec -it boiler-auth /bin/bash
   docker logs boiler-auth --follow
   ```

This comprehensive Phase 8 plan will give you a production-ready Docker setup while maintaining excellent developer experience. The multi-stage builds ensure small production images, while the override configuration enables hot reload during development.
