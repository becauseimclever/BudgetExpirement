# Build Docker image for ARM64 (Raspberry Pi) on Windows
# This script uses Docker buildx for cross-platform compilation
# Prerequisites: Docker Desktop with buildx support

$ErrorActionPreference = 'Stop'

# Configuration
$ImageName = "budgetexperiment"
$ImageTag = "latest"
$PublishDir = "C:\ws\BudgetExpirement\publish"
$ApiProject = "C:\ws\BudgetExpirement\src\BudgetExperiment.Api\BudgetExperiment.Api.csproj"

Write-Host "========================================"  -ForegroundColor Cyan
Write-Host "Building Docker Image for Raspberry Pi" -ForegroundColor Cyan
Write-Host "========================================"  -ForegroundColor Cyan
Write-Host ""

# Step 1: Run tests
Write-Host "[1/5] Running tests..." -ForegroundColor Yellow
try {
    dotnet test C:\ws\BudgetExpirement\BudgetExperiment.sln --configuration Release
    Write-Host "✓ All tests passed!" -ForegroundColor Green
} catch {
    Write-Host "✗ Tests failed. Aborting build." -ForegroundColor Red
    exit 1
}
Write-Host ""

# Step 2: Clean and publish for linux-arm64
Write-Host "[2/5] Publishing application for linux-arm64..." -ForegroundColor Yellow
if (Test-Path $PublishDir) {
    Remove-Item -Recurse -Force $PublishDir
}

try {
    dotnet publish $ApiProject `
        --configuration Release `
        --runtime linux-arm64 `
        --self-contained true `
        --output $PublishDir `
        /p:PublishTrimmed=false
    Write-Host "✓ Application published successfully to $PublishDir" -ForegroundColor Green
} catch {
    Write-Host "✗ Publishing failed. Aborting build." -ForegroundColor Red
    exit 1
}
Write-Host ""

# Step 3: Setup Docker buildx for cross-platform builds
Write-Host "[3/5] Setting up Docker buildx for ARM64..." -ForegroundColor Yellow
try {
    # Check if buildx builder exists, create if not
    $builderExists = docker buildx ls | Select-String "multiarch"
    if (-not $builderExists) {
        Write-Host "Creating multiarch builder..." -ForegroundColor Gray
        docker buildx create --name multiarch --driver docker-container --use
        docker buildx inspect --bootstrap
    } else {
        Write-Host "Using existing multiarch builder..." -ForegroundColor Gray
        docker buildx use multiarch
    }
    Write-Host "✓ Buildx configured successfully" -ForegroundColor Green
} catch {
    Write-Host "⚠ Buildx setup encountered issues, continuing with default builder..." -ForegroundColor Yellow
}
Write-Host ""

# Step 4: Build Docker image for ARM64
Write-Host "[4/5] Building Docker image for ARM64..." -ForegroundColor Yellow
Write-Host "This may take several minutes on first build..." -ForegroundColor Gray
try {
    docker buildx build `
        --platform linux/arm64 `
        --tag "${ImageName}:${ImageTag}" `
        --load `
        .
    Write-Host "✓ Docker image built successfully!" -ForegroundColor Green
} catch {
    Write-Host "✗ Docker build failed." -ForegroundColor Red
    exit 1
}
Write-Host ""

# Step 5: Show image details
Write-Host "[5/5] Docker image details:" -ForegroundColor Yellow
docker images "${ImageName}:${ImageTag}"
Write-Host ""

Write-Host "========================================"  -ForegroundColor Green
Write-Host "Build Complete!" -ForegroundColor Green
Write-Host "========================================"  -ForegroundColor Green
Write-Host ""
Write-Host "Docker image: ${ImageName}:${ImageTag}" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Save image:     .\save-docker-image.ps1" -ForegroundColor White
Write-Host "  2. Transfer to Pi: .\transfer-to-pi.ps1 -PiHost your-pi-hostname" -ForegroundColor White
Write-Host "  3. Deploy on Pi:   ssh to Pi and run ./load-and-deploy.sh" -ForegroundColor White
Write-Host ""
Write-Host "Or use the all-in-one script:" -ForegroundColor Yellow
Write-Host "  .\deploy-to-pi.ps1 -PiHost your-pi-hostname -PiUser pi" -ForegroundColor White
Write-Host ""
