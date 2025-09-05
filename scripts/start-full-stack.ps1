param(
    [switch]$NoCache,
    [switch]$Recreate,
    [int]$HealthTimeoutSeconds = 240,
    [int]$HealthPollSeconds = 10
)

Write-Host "Starting Complete Boiler Stack (HTTPS + Services + Frontend)" -ForegroundColor Green

function Show-MonitoringCredentials {
    $configPathPrimary = "src/services/UserService/appsettings.json"
    $configPathDev     = "src/services/UserService/appsettings.Development.json"
    $configPath        = if (Test-Path $configPathDev) { $configPathDev } elseif (Test-Path $configPathPrimary) { $configPathPrimary } else { $null }

    if (-not $configPath) {
        Write-Host "  (Monitoring) Config file not found." -ForegroundColor DarkYellow
        return
    }

    try {
        $json = Get-Content -Raw -Path $configPath | ConvertFrom-Json
        $mon  = $json.Monitoring
        if ($mon -and $mon.Enabled -eq $true) {
            $email = if ([string]::IsNullOrWhiteSpace($env:MONITORING_EMAIL)) { $mon.Email } else { $env:MONITORING_EMAIL }
            $pwd   = if ([string]::IsNullOrWhiteSpace($env:MONITORING_PASSWORD)) { $mon.Password } else { $env:MONITORING_PASSWORD }

            Write-Host "Monitoring (Swagger metrics) account:" -ForegroundColor Cyan
            Write-Host "  Email:    $email" -ForegroundColor Gray
            Write-Host "  Password: $pwd" -ForegroundColor Gray
            Write-Host "  Obtain JWT via AuthService login → authorize in UserService Swagger." -ForegroundColor DarkGray
        } else {
            Write-Host "Monitoring user seeding disabled (Monitoring.Enabled != true)" -ForegroundColor DarkYellow
        }
    }
    catch {
        Write-Host "  (Monitoring) Failed to parse monitoring config: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Ensure Docker running
try { docker info | Out-Null } catch {
    Write-Host "Docker is not running. Start Docker Desktop first." -ForegroundColor Red
    exit 1
}

# SSL cert bootstrap
if (-not (Test-Path "docker\nginx\ssl\localhost.crt") -or -not (Test-Path "docker\nginx\ssl\localhost.key")) {
    Write-Host "SSL certificates not found. Creating self-signed cert..." -ForegroundColor Yellow
    New-Item -ItemType Directory -Force -Path "docker\nginx\ssl" | Out-Null
    docker run --rm -v "${PWD}\docker\nginx\ssl:/certs" --workdir /certs alpine/openssl `
        req -x509 -nodes -days 365 -newkey rsa:2048 -keyout localhost.key -out localhost.crt -subj "/CN=localhost"
    Write-Host "SSL certificates created." -ForegroundColor Green
}

$composeFile = "docker/docker-compose.yml"
$envFile     = ".env"

if (-not (Test-Path $composeFile)) {
    Write-Host "Compose file '$composeFile' not found." -ForegroundColor Red
    exit 1
}

# Optional full recreate
if ($Recreate) {
    Write-Host "Recreating stack (docker-compose down)..." -ForegroundColor Yellow
    docker-compose -f $composeFile --env-file $envFile down --remove-orphans
}

# Build phase (handle cache properly)
if ($NoCache -or $env:NO_CACHE_DOCKER -eq "1") {
    Write-Host "No-cache build (docker-compose build --no-cache)..." -ForegroundColor Yellow
    docker-compose -f $composeFile --env-file $envFile build --no-cache
} else {
    Write-Host "Incremental build (docker-compose build)..." -ForegroundColor Yellow
    docker-compose -f $composeFile --env-file $envFile build
}

# Up (do not pass --no-cache here)
Write-Host "Starting containers (docker-compose up -d)..." -ForegroundColor Yellow
docker-compose -f $composeFile --env-file $envFile up -d

# Health wait
Write-Host "Waiting for services to report healthy..." -ForegroundColor Yellow

# Expected healthy count (adjust if you add/remove services)
$expected = 7

$elapsed = 0
while ($elapsed -lt $HealthTimeoutSeconds) {
    $healthy = (docker ps --filter "health=healthy" --format "{{.Names}}" | Measure-Object -Line).Lines
    Write-Host ("  Healthy: {0}/{1} (t={2}s)" -f $healthy, $expected, $elapsed) -ForegroundColor Gray
    if ($healthy -ge $expected) { break }
    Start-Sleep -Seconds $HealthPollSeconds
    $elapsed += $HealthPollSeconds
}

if ($elapsed -ge $HealthTimeoutSeconds) {
    Write-Host "Timeout waiting for services. Current statuses:" -ForegroundColor Red
    docker-compose -f $composeFile --env-file $envFile ps
    Write-Host "Hint: Check individual logs, e.g. docker-compose -f $composeFile logs workflow-service" -ForegroundColor Yellow
    exit 2
}

Write-Host "Complete stack started successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Services available:" -ForegroundColor Cyan
Write-Host "  Frontend HTTPS:        https://localhost:3000" -ForegroundColor White
Write-Host "  AuthService Swagger:   https://localhost:7001/swagger" -ForegroundColor White
Write-Host "  UserService Swagger:   https://localhost:7002/swagger" -ForegroundColor White
Write-Host "  WorkflowService Swagger:https://localhost:7003/swagger" -ForegroundColor White
Write-Host "  API Gateway Info:      https://localhost:7000/gateway/info" -ForegroundColor White
Write-Host "  PgAdmin:               http://localhost:8080" -ForegroundColor White
Write-Host ""
Write-Host "Sample tenant admin:" -ForegroundColor Cyan
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
Write-Host "  View logs (all): docker-compose -f $composeFile logs -f" -ForegroundColor Gray
Write-Host "  Stop stack:      .\scripts\stop-full-stack.ps1" -ForegroundColor Gray
Write-Host "  Status:          docker-compose -f $composeFile ps" -ForegroundColor Gray
Write-Host "  Restart one:     docker-compose -f $composeFile restart <service>" -ForegroundColor Gray
Write-Host "  Workflow logs:   docker-compose -f $composeFile logs -f workflow-service" -ForegroundColor Gray
Write-Host ""
Write-Host "If troubleshooting DI, search logs for 'DI_DIAG' (add console write in Program.cs)." -ForegroundColor DarkYellow
Write-Host ""
