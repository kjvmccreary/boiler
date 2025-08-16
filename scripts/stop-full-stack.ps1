Write-Host "Stopping Complete Boiler Stack..." -ForegroundColor Yellow

docker-compose -f docker/docker-compose.yml down

Write-Host "Complete stack stopped successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "To start again: .\scripts\start-full-stack.ps1" -ForegroundColor Cyan
