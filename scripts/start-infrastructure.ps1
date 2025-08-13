# Start Boiler development infrastructure
Write-Host "🚀 Starting Boiler development infrastructure..." -ForegroundColor Green

# Check if Docker is running
try {
    docker info | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "Docker not accessible"
    }
} catch {
    Write-Host "❌ Docker is not running. Please start Docker Desktop first." -ForegroundColor Red
    exit 1
}

# Start infrastructure services
Write-Host "🐳 Starting containers..." -ForegroundColor Cyan
docker compose -f docker/docker-compose.infrastructure.yml up -d

# Wait for services to be ready
Write-Host "⏳ Waiting for services to be ready..." -ForegroundColor Yellow
Start-Sleep -Seconds 15

# Test database connection with retries
$attempts = 0
$maxAttempts = 6

do {
    $attempts++
    Write-Host "  Testing connection attempt $attempts/$maxAttempts..." -ForegroundColor White
    
    try {
        $testResult = docker exec boiler-postgres psql -U boiler_app -d boiler_dev -c "SELECT 'Boiler DB Ready!' as status;" 2>$null
        if ($LASTEXITCODE -eq 0) {
            Write-Host "  ✅ PostgreSQL is ready!" -ForegroundColor Green
            Write-Host "  📊 Database response:" -ForegroundColor Cyan
            Write-Host "$testResult" -ForegroundColor White
            break
        }
    } catch {
        # Continue waiting
    }
    
    if ($attempts -lt $maxAttempts) {
        Start-Sleep -Seconds 5
    }
} while ($attempts -lt $maxAttempts)

if ($attempts -eq $maxAttempts) {
    Write-Host "  ⚠️  Database may still be starting up..." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "🔗 Boiler Connection Details:" -ForegroundColor Cyan
Write-Host "  PostgreSQL: localhost:5432" -ForegroundColor White
Write-Host "  Database: boiler_dev" -ForegroundColor White
Write-Host "  Username: boiler_app" -ForegroundColor White
Write-Host "  Password: dev_password123" -ForegroundColor White
Write-Host ""
Write-Host "  Redis: localhost:6379" -ForegroundColor White
Write-Host ""
Write-Host "  PgAdmin: http://localhost:8080" -ForegroundColor White
Write-Host "  Email: admin@example.com" -ForegroundColor White
Write-Host "  Password: admin123" -ForegroundColor White
Write-Host ""
Write-Host "✅ Boiler infrastructure started successfully!" -ForegroundColor Green
