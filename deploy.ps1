# Deployment script for BudgetExperiment
# Runs all tests, then publishes self-contained builds for Windows x64 and Linux ARM64

$ErrorActionPreference = 'Stop'

Write-Host "Running all tests..." -ForegroundColor Cyan

dotnet test C:\ws\BudgetExpirement\BudgetExperiment.sln

Write-Host "All tests passed. Publishing self-contained builds..." -ForegroundColor Green

$apiProject = "C:\ws\BudgetExpirement\src\BudgetExperiment.Api\BudgetExperiment.Api.csproj"
$publishDir = "C:\ws\BudgetExpirement\publish"

# Windows x64
Write-Host "Publishing for win-x64..." -ForegroundColor Yellow
dotnet publish $apiProject -c Release -r win-x64 --self-contained true -o "$publishDir\win-x64"

# Linux ARM64
Write-Host "Publishing for linux-arm64..." -ForegroundColor Yellow
dotnet publish $apiProject -c Release -r linux-arm64 --self-contained true -o "$publishDir\linux-arm64"

Write-Host "Deployment complete. Output in $publishDir" -ForegroundColor Green
