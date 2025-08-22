Write-Host "🔄 Restarting Complete Boiler Stack..." -ForegroundColor Yellow

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
            $email = $env:MONITORING_EMAIL
            if ([string]::IsNullOrWhiteSpace($email)) { $email = $mon.Email }
            $pwd = $env:MONITORING_PASSWORD
            if ([string]::IsNullOrWhiteSpace($pwd)) { $pwd = $mon.Password }

            Write-Host "Monitoring (Swagger metrics) account:" -ForegroundColor Cyan
            Write-Host "  Email:    $email" -ForegroundColor Gray
            Write-Host "  Password: $pwd" -ForegroundColor Gray
        } else {
            Write-Host "Monitoring user seeding disabled (Monitoring.Enabled != true)" -ForegroundColor DarkYellow
        }
    }
    catch {
        Write-Host "  (Monitoring) Failed to parse monitoring config: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "Stopping services..." -ForegroundColor Gray
docker-compose -f docker/docker-compose.yml down

Start-Sleep -Seconds 5

Write-Host "Starting services..." -ForegroundColor Gray
docker-compose -f docker/docker-compose.yml --env-file .env up -d --build

Write-Host "⏳ Waiting for services to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 45

Write-Host "✅ Complete stack restarted successfully!" -ForegroundColor Green
Show-MonitoringCredentials
