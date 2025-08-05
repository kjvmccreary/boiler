# Reset Boiler database (removes all data)
Write-Host "⚠️  WARNING: This will delete ALL Boiler database data!" -ForegroundColor Red
$confirmation = Read-Host "Type 'RESET' to confirm"

if ($confirmation -eq "RESET") {
    Write-Host "🗑️  Resetting Boiler database..." -ForegroundColor Yellow
    
    # Stop and remove containers with volumes
    docker compose -f docker/docker-compose.infrastructure.yml down -v
    
    # Wait a moment
    Start-Sleep -Seconds 3
    
    # Start fresh
    docker compose -f docker/docker-compose.infrastructure.yml up -d
    
    Write-Host "✅ Boiler database reset completed!" -ForegroundColor Green
    Write-Host "⏳ Give it 30 seconds to fully initialize..." -ForegroundColor Yellow
} else {
    Write-Host "❌ Reset cancelled." -ForegroundColor Green
}
