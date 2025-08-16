Write-Host "🚀 Starting Complete Boiler Stack with HTTPS..." -ForegroundColor Green

# Start everything
docker-compose -f docker/docker-compose.yml --env-file docker/.env up -d --build

Write-Host "⏳ Waiting for services to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 45

Write-Host "✅ Stack started! Services available:" -ForegroundColor Green
Write-Host "  🔐 AuthService:    https://localhost:7001/swagger" -ForegroundColor Cyan
Write-Host "  👤 UserService:    https://localhost:7002/swagger" -ForegroundColor Cyan  
Write-Host "  🚪 API Gateway:    https://localhost:7000/gateway/info" -ForegroundColor Cyan
Write-Host "  🗄️  PgAdmin:        http://localhost:8080" -ForegroundColor Cyan
Write-Host ""
Write-Host "📋 View logs: docker-compose -f docker/docker-compose.yml logs -f" -ForegroundColor Yellow
Write-Host "🛑 Stop all: docker-compose -f docker/docker-compose.yml down" -ForegroundColor Yellow
