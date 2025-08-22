Write-Host "Starting Complete Boiler Stack (HTTPS + Services + Frontend)" -ForegroundColor Green

# Helper: Show monitoring credentials (non-prod convenience)
function Show-MonitoringCredentials {
    $configPathPrimary = "src/services/UserService/appsettings.json"
    $configPathDev = "src/services/UserService/appsettings.Development.json"

    $configPath = if (Test-Path $configPathDev) { $configPathDev } elseif (Test-Path $configPathPrimary) { $configPathPrimary } else { $null }

    if (-not $configPath) {
        Write-Host "  (Monitoring) Config file not found." -ForegroundColor DarkYellow
        return
    }

    try {
        $json = Get-Content -Raw -Path $configPath | ConvertFrom-Json
        $mon = $json.Monitoring
        if ($mon -and $mon.Enabled -eq $true) {
            # Allow environment overrides (recommended for password)
            $email = $env:MONITORING_EMAIL
            if ([string]::IsNullOrWhiteSpace($email)) { $email = $mon.Email }
            $pwd = $env:MONITORING_PASSWORD
            if ([string]::IsNullOrWhiteSpace($pwd)) { $pwd = $mon.Password }

            Write-Host "Monitoring (Swagger metrics) account:" -ForegroundColor Cyan
            Write-Host "  Email:    $email" -ForegroundColor Gray
            Write-Host "  Password: $pwd" -ForegroundColor Gray
            Write-Host "  NOTE: Obtain JWT via AuthService login, then authorize in UserService Swagger to call /api/admin/cache/* and /api/admin/performance/* read endpoints." -ForegroundColor DarkGray
        } else {
            Write-Host "Monitoring user seeding disabled (Monitoring.Enabled != true)" -ForegroundColor DarkYellow
        }
    }
    catch {
        Write-Host "  (Monitoring) Failed to parse monitoring config: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Check if Docker is running
try {
    docker info | Out-Null
} catch {
    Write-Host "Docker is not running. Please start Docker Desktop." -ForegroundColor Red
    exit 1
}

# Check for required SSL certificates
if (-not (Test-Path "docker\nginx\ssl\localhost.crt") -or -not (Test-Path "docker\nginx\ssl\localhost.key")) {
    Write-Host "SSL certificates not found. Creating them now..." -ForegroundColor Yellow
    
    New-Item -ItemType Directory -Force -Path "docker\nginx\ssl" | Out-Null
    
    docker run --rm -v "${PWD}\docker\nginx\ssl:/certs" --workdir /certs alpine/openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout localhost.key -out localhost.crt -subj "/CN=localhost"
    
    Write-Host "SSL certificates created successfully!" -ForegroundColor Green
}

Write-Host "Building and starting all services..." -ForegroundColor Yellow
docker-compose -f docker/docker-compose.yml --env-file .env up -d --build

Write-Host "Waiting for services to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 50

Write-Host "Checking service health..." -ForegroundColor Yellow
$healthyServices = 0
$maxRetries = 6
$retryCount = 0

do {
    $healthyServices = (docker ps --filter "health=healthy" --format "table {{.Names}}" | Measure-Object -Line).Lines - 1
    if ($healthyServices -lt 6) {
        Write-Host "  $healthyServices/6 services healthy. Waiting..." -ForegroundColor Yellow
        Start-Sleep -Seconds 10
        $retryCount++
    }
} while ($healthyServices -lt 6 -and $retryCount -lt $maxRetries)

Write-Host "Complete stack started successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Services available:" -ForegroundColor Cyan
Write-Host "  Frontend HTTPS:       https://localhost:3000" -ForegroundColor White
Write-Host "  AuthService HTTPS:    https://localhost:7001/swagger" -ForegroundColor White
Write-Host "  UserService HTTPS:    https://localhost:7002/swagger" -ForegroundColor White  
Write-Host "  API Gateway HTTPS:    https://localhost:7000/gateway/info" -ForegroundColor White
Write-Host "  PgAdmin:              http://localhost:8080" -ForegroundColor White
Write-Host ""
Write-Host "Test credentials (sample tenant admin):" -ForegroundColor Cyan
Write-Host "  Email:    admin@tenant1.com" -ForegroundColor Gray
Write-Host "  Password: Admin123!" -ForegroundColor Gray
Write-Host ""
Write-Host "Monitoring credentials:" -ForegroundColor Cyan
Write-Host "  Email:    monitor@local" -ForegroundColor Gray
Write-Host "  Password: ChangeMe123!" -ForegroundColor Gray
Write-Host ""
Show-MonitoringCredentials
Write-Host ""
Write-Host "Useful commands:" -ForegroundColor Yellow
Write-Host "  View logs:    docker-compose -f docker/docker-compose.yml logs -f" -ForegroundColor Gray
Write-Host "  Stop stack:   .\scripts\stop-full-stack.ps1" -ForegroundColor Gray
Write-Host "  Check status: docker-compose -f docker/docker-compose.yml ps" -ForegroundColor Gray
Write-Host "  Restart svc:  docker-compose -f docker/docker-compose.yml restart <service-name>" -ForegroundColor Gray
