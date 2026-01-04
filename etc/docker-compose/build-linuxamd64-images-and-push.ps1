param ($version='latest')

$currentFolder = $PSScriptRoot
$slnFolder = Join-Path $currentFolder "../../"

Write-Host "********* BUILDING DbMigrator *********" -ForegroundColor Green
$dbMigratorFolder = Join-Path $slnFolder "src/HC.DbMigrator"
Set-Location $dbMigratorFolder
Write-Host "Publishing DbMigrator..." -ForegroundColor Yellow
& dotnet publish -c Release -o bin/Release/net10.0/publish
$publishExitCode = $LASTEXITCODE
Start-Sleep -Milliseconds 500  # Wait for file system to sync
$publishPath = Join-Path (Get-Location) "bin/Release/net10.0/publish"
if ($publishExitCode -ne 0) {
    Write-Host "ERROR: dotnet publish failed for DbMigrator (Exit Code: $publishExitCode)" -ForegroundColor Red
    exit 1
}
if (-not (Test-Path $publishPath)) {
    Write-Host "ERROR: Publish folder not found: $publishPath" -ForegroundColor Red
    Write-Host "Current directory: $(Get-Location)" -ForegroundColor Yellow
    exit 1
}
Write-Host "Publish successful. Output: $publishPath" -ForegroundColor Green
Write-Host "Building Docker image for DbMigrator..." -ForegroundColor Yellow
& docker buildx build --platform linux/amd64 -f Dockerfile.local -t longnguyen1331/hc-db-migrator:$version . --push
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Docker build failed for DbMigrator (Exit Code: $LASTEXITCODE)" -ForegroundColor Red
    exit 1
}


Write-Host "********* BUILDING Blazor Application *********" -ForegroundColor Green
$blazorFolder = Join-Path $slnFolder "src/HC.Blazor"
Set-Location $blazorFolder
Write-Host "Publishing Blazor..." -ForegroundColor Yellow
& dotnet publish -c Release -o bin/Release/net10.0/publish
$publishExitCode = $LASTEXITCODE
Start-Sleep -Milliseconds 500  # Wait for file system to sync
$publishPath = Join-Path (Get-Location) "bin/Release/net10.0/publish"
if ($publishExitCode -ne 0) {
    Write-Host "ERROR: dotnet publish failed for Blazor (Exit Code: $publishExitCode)" -ForegroundColor Red
    exit 1
}
if (-not (Test-Path $publishPath)) {
    Write-Host "ERROR: Publish folder not found: $publishPath" -ForegroundColor Red
    Write-Host "Current directory: $(Get-Location)" -ForegroundColor Yellow
    exit 1
}
Write-Host "Publish successful. Output: $publishPath" -ForegroundColor Green
Write-Host "Building Docker image for Blazor..." -ForegroundColor Yellow
& docker buildx build --platform linux/amd64 -f Dockerfile.local -t longnguyen1331/hc-blazor:$version . --push
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Docker build failed for Blazor (Exit Code: $LASTEXITCODE)" -ForegroundColor Red
    exit 1
}

Write-Host "********* BUILDING Api.Host Application *********" -ForegroundColor Green
$hostFolder = Join-Path $slnFolder "src/HC.HttpApi.Host"
Set-Location $hostFolder
Write-Host "Publishing Api.Host..." -ForegroundColor Yellow
& dotnet publish -c Release -o bin/Release/net10.0/publish
$publishExitCode = $LASTEXITCODE
Start-Sleep -Milliseconds 500  # Wait for file system to sync
$publishPath = Join-Path (Get-Location) "bin/Release/net10.0/publish"
if ($publishExitCode -ne 0) {
    Write-Host "ERROR: dotnet publish failed for Api.Host (Exit Code: $publishExitCode)" -ForegroundColor Red
    exit 1
}
if (-not (Test-Path $publishPath)) {
    Write-Host "ERROR: Publish folder not found: $publishPath" -ForegroundColor Red
    Write-Host "Current directory: $(Get-Location)" -ForegroundColor Yellow
    exit 1
}
Write-Host "Publish successful. Output: $publishPath" -ForegroundColor Green
Write-Host "Building Docker image for Api.Host..." -ForegroundColor Yellow
& docker buildx build --platform linux/amd64 -f Dockerfile.local -t longnguyen1331/hc-api:$version . --push
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Docker build failed for Api.Host (Exit Code: $LASTEXITCODE)" -ForegroundColor Red
    exit 1
}

Write-Host "********* BUILDING AuthServer Application *********" -ForegroundColor Green
$authServerAppFolder = Join-Path $slnFolder "src/HC.AuthServer"
Set-Location $authServerAppFolder
Write-Host "Publishing AuthServer..." -ForegroundColor Yellow
& dotnet publish -c Release -o bin/Release/net10.0/publish
$publishExitCode = $LASTEXITCODE
Start-Sleep -Milliseconds 500  # Wait for file system to sync
$publishPath = Join-Path (Get-Location) "bin/Release/net10.0/publish"
if ($publishExitCode -ne 0) {
    Write-Host "ERROR: dotnet publish failed for AuthServer (Exit Code: $publishExitCode)" -ForegroundColor Red
    exit 1
}
if (-not (Test-Path $publishPath)) {
    Write-Host "ERROR: Publish folder not found: $publishPath" -ForegroundColor Red
    Write-Host "Current directory: $(Get-Location)" -ForegroundColor Yellow
    exit 1
}
Write-Host "Publish successful. Output: $publishPath" -ForegroundColor Green
Write-Host "Building Docker image for AuthServer..." -ForegroundColor Yellow
& docker buildx build --platform linux/amd64 -f Dockerfile.local -t longnguyen1331/hc-authserver:$version . --push
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Docker build failed for AuthServer (Exit Code: $LASTEXITCODE)" -ForegroundColor Red
    exit 1
}


### ALL COMPLETED
Write-Host "COMPLETED" -ForegroundColor Green
Set-Location $currentFolder
exit $LASTEXITCODE