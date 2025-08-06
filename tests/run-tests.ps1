Write-Host "Running AuthService Unit Tests..." -ForegroundColor Green
Write-Host "==================================" -ForegroundColor Green

# Navigate to the test directory
Set-Location -Path "$PSScriptRoot\unit\AuthService.Tests"

# Run tests with coverage
dotnet test --logger "trx;LogFileName=TestResults.trx" `
           --logger "console;verbosity=detailed" `
           --collect:"XPlat Code Coverage" `
           --results-directory ./TestResults

Write-Host ""
Write-Host "Test run completed!" -ForegroundColor Green
Write-Host "Results saved to: $(Get-Location)\TestResults" -ForegroundColor Yellow
