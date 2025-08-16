Write-Host "🔄 Restarting Complete Boiler Stack..." -ForegroundColor Yellow

# Stop everything
Write-Host "Stopping services..." -ForegroundColor Gray
docker-compose -f docker/docker-compose.yml down

# Wait a moment
Start-Sleep -Seconds 5

# Start everything
Write-Host "Starting services..." -ForegroundColor Gray
docker-compose -f docker/docker-compose.yml --env-file .env up -d --build

Write-Host "⏳ Waiting for services to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 45

Write-Host "✅ Complete stack restarted successfully!" -ForegroundColor Green
