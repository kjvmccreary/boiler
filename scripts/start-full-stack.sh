#!/bin/bash

echo "🚀 Starting Complete Boiler Stack (HTTPS + Services)"

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker is not running. Please start Docker."
    exit 1
fi

echo "🔨 Building and starting all services..."
docker-compose -f docker/docker-compose.yml --env-file .env up -d --build

echo "⏳ Waiting for services to start..."
sleep 45

echo "✅ Complete stack started successfully!"
echo ""
echo "🌐 Services available:"
echo "  🔐 AuthService HTTPS:    https://localhost:7001/swagger"
echo "  👤 UserService HTTPS:    https://localhost:7002/swagger"  
echo "  🚪 API Gateway HTTPS:    https://localhost:7000/gateway/info"
echo "  🗄️  PgAdmin:              http://localhost:8080"
echo ""
echo "📋 Useful commands:"
echo "  View logs:    docker-compose -f docker/docker-compose.yml logs -f"
echo "  Stop stack:   ./scripts/stop-full-stack.sh"
echo "  Check status: docker-compose -f docker/docker-compose.yml ps"
