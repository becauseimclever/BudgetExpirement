# Complete deployment script: Build on Windows, transfer to Raspberry Pi, and deploy
# Prerequisites: Docker Desktop, SSH access to Raspberry Pi, SCP available

param(
    [Parameter(Mandatory=$true)]
    [string]$PiHost,
    
    [Parameter(Mandatory=$false)]
    [string]$PiUser = "pi",
    
    [Parameter(Mandatory=$false)]
    [string]$PiDeployPath = "/home/$PiUser/BudgetExperiment",
    
    [Parameter(Mandatory=$false)]
    [string]$ImageName = "budgetexperiment",
    
    [Parameter(Mandatory=$false)]
    [string]$ImageTag = "latest"
)

$ErrorActionPreference = 'Stop'

Write-Host "========================================"  -ForegroundColor Cyan
Write-Host "Complete Windows to Pi Deployment" -ForegroundColor Cyan
Write-Host "========================================"  -ForegroundColor Cyan
Write-Host ""
Write-Host "Target: ${PiUser}@${PiHost}:${PiDeployPath}" -ForegroundColor Gray
Write-Host "Image:  ${ImageName}:${ImageTag}" -ForegroundColor Gray
Write-Host ""

# Step 1: Build Docker image
Write-Host "[Step 1/4] Building Docker image on Windows..." -ForegroundColor Yellow
& "C:\ws\BudgetExpirement\build-docker-windows.ps1"
if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Build failed. Aborting deployment." -ForegroundColor Red
    exit 1
}
Write-Host ""

# Step 2: Save Docker image to tar file
Write-Host "[Step 2/4] Saving Docker image to tar file..." -ForegroundColor Yellow
$ImageFile = "C:\ws\BudgetExpirement\budgetexperiment-image.tar"
if (Test-Path $ImageFile) {
    Remove-Item -Force $ImageFile
}

try {
    docker save -o $ImageFile "${ImageName}:${ImageTag}"
    $FileSizeMB = [math]::Round((Get-Item $ImageFile).Length / 1MB, 2)
    Write-Host "✓ Image saved successfully ($FileSizeMB MB)" -ForegroundColor Green
} catch {
    Write-Host "✗ Failed to save Docker image." -ForegroundColor Red
    exit 1
}
Write-Host ""

# Step 3: Transfer files to Raspberry Pi
Write-Host "[Step 3/4] Transferring files to Raspberry Pi..." -ForegroundColor Yellow
Write-Host "This may take several minutes depending on your network speed..." -ForegroundColor Gray

try {
    # Ensure remote directory exists
    Write-Host "Creating remote directory..." -ForegroundColor Gray
    ssh "${PiUser}@${PiHost}" "mkdir -p ${PiDeployPath}"
    
    # Transfer Docker image
    Write-Host "Transferring Docker image (this may take a while)..." -ForegroundColor Gray
    scp -C $ImageFile "${PiUser}@${PiHost}:${PiDeployPath}/budgetexperiment-image.tar"
    
    # Transfer docker-compose.yml
    Write-Host "Transferring docker-compose.yml..." -ForegroundColor Gray
    scp "C:\ws\BudgetExpirement\docker-compose.yml" "${PiUser}@${PiHost}:${PiDeployPath}/"
    
    # Transfer load-and-deploy.sh script
    Write-Host "Transferring deployment script..." -ForegroundColor Gray
    scp "C:\ws\BudgetExpirement\load-and-deploy.sh" "${PiUser}@${PiHost}:${PiDeployPath}/"
    
    # Make script executable
    ssh "${PiUser}@${PiHost}" "chmod +x ${PiDeployPath}/load-and-deploy.sh"
    
    Write-Host "✓ Files transferred successfully!" -ForegroundColor Green
} catch {
    Write-Host "✗ Failed to transfer files to Raspberry Pi." -ForegroundColor Red
    Write-Host "Error: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting tips:" -ForegroundColor Yellow
    Write-Host "  1. Verify SSH access: ssh ${PiUser}@${PiHost}" -ForegroundColor White
    Write-Host "  2. Check network connectivity to Raspberry Pi" -ForegroundColor White
    Write-Host "  3. Ensure SSH key is set up or you have password access" -ForegroundColor White
    exit 1
}
Write-Host ""

# Clean up local tar file
Write-Host "Cleaning up local tar file..." -ForegroundColor Gray
Remove-Item -Force $ImageFile
Write-Host ""

# Step 4: Deploy on Raspberry Pi
Write-Host "[Step 4/4] Deploying on Raspberry Pi..." -ForegroundColor Yellow
Write-Host "Connecting to Pi and running deployment..." -ForegroundColor Gray

try {
    ssh "${PiUser}@${PiHost}" "cd ${PiDeployPath} && ./load-and-deploy.sh"
    Write-Host "✓ Deployment completed successfully!" -ForegroundColor Green
} catch {
    Write-Host "✗ Deployment failed on Raspberry Pi." -ForegroundColor Red
    Write-Host "You can manually deploy by running:" -ForegroundColor Yellow
    Write-Host "  ssh ${PiUser}@${PiHost}" -ForegroundColor White
    Write-Host "  cd ${PiDeployPath}" -ForegroundColor White
    Write-Host "  ./load-and-deploy.sh" -ForegroundColor White
    exit 1
}
Write-Host ""

Write-Host "========================================"  -ForegroundColor Green
Write-Host "Deployment Complete!" -ForegroundColor Green
Write-Host "========================================"  -ForegroundColor Green
Write-Host ""
Write-Host "Application should be running at:" -ForegroundColor Cyan
Write-Host "  http://${PiHost}:5099" -ForegroundColor White
Write-Host ""
Write-Host "Useful commands (run via SSH):" -ForegroundColor Yellow
Write-Host "  ssh ${PiUser}@${PiHost}" -ForegroundColor White
Write-Host "  cd ${PiDeployPath}" -ForegroundColor White
Write-Host "  docker-compose logs -f              # View logs" -ForegroundColor White
Write-Host "  docker-compose ps                   # Check status" -ForegroundColor White
Write-Host "  docker-compose restart              # Restart app" -ForegroundColor White
Write-Host "  curl http://localhost:5099/health   # Check health" -ForegroundColor White
Write-Host ""
