# Stop Boiler development infrastructure
Write-Host "🛑 Stopping Boiler development infrastructure..." -ForegroundColor Yellow
docker compose -f docker/docker-compose.infrastructure.yml down
Write-Host "✅ Boiler infrastructure stopped successfully!" -ForegroundColor Green
