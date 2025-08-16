Write-Host "Starting Complete Boiler Stack (HTTPS + Services)" -ForegroundColor Green

# Check if Docker is running
try {
    docker info | Out-Null
} catch {
    Write-Host "Docker is not running. Please start Docker Desktop." -ForegroundColor Red
    exit 1
}

Write-Host "Building and starting all services..." -ForegroundColor Yellow
docker-compose -f docker/docker-compose.yml --env-file .env up -d --build

Write-Host "Waiting for services to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 45

Write-Host "Complete stack started successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Services available:" -ForegroundColor Cyan
Write-Host "  AuthService HTTPS:    https://localhost:7001/swagger" -ForegroundColor White
Write-Host "  UserService HTTPS:    https://localhost:7002/swagger" -ForegroundColor White  
Write-Host "  API Gateway HTTPS:    https://localhost:7000/gateway/info" -ForegroundColor White
Write-Host "  PgAdmin:              http://localhost:8080" -ForegroundColor White
Write-Host ""
Write-Host "Useful commands:" -ForegroundColor Yellow
Write-Host "  View logs:    docker-compose -f docker/docker-compose.yml logs -f" -ForegroundColor Gray
Write-Host "  Stop stack:   .\scripts\stop-full-stack.ps1" -ForegroundColor Gray
Write-Host "  Check status: docker-compose -f docker/docker-compose.yml ps" -ForegroundColor Gray
