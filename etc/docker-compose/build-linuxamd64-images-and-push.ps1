param ($version='latest')

$currentFolder = $PSScriptRoot
$slnFolder = Join-Path $currentFolder "../../"

Write-Host "********* BUILDING DbMigrator *********" -ForegroundColor Green
$dbMigratorFolder = Join-Path $slnFolder "src/HC.DbMigrator"
Set-Location $dbMigratorFolder
Write-Host "Publishing DbMigrator..." -ForegroundColor Yellow
try {
    $result = dotnet publish -c Release -o bin/Release/net10.0/publish 2>&1
    if (-not $?) {
        throw "dotnet publish failed"
    }
} catch {
    Write-Host "ERROR: dotnet publish failed for DbMigrator" -ForegroundColor Red
    Write-Host $result -ForegroundColor Red
    exit 1
}
Start-Sleep -Seconds 1  # Wait for file system to sync
$currentDir = Get-Location
$publishPath = Join-Path $currentDir "bin/Release/net10.0/publish"
$publishPathFull = [System.IO.Path]::GetFullPath($publishPath)
if (-not (Test-Path $publishPathFull)) {
    Write-Host "ERROR: Publish folder not found: $publishPathFull" -ForegroundColor Red
    Write-Host "Current directory: $currentDir" -ForegroundColor Yellow
    exit 1
}
Write-Host "Publish successful. Output: $publishPathFull" -ForegroundColor Green
Write-Host "Building Docker image for DbMigrator (linux/amd64)..." -ForegroundColor Yellow
try {
    docker buildx build --platform linux/amd64 -f Dockerfile.local -t longnguyen1331/hc-db-migrator:$version -t longnguyen1331/hc-db-migrator:latest . --push
    if (-not $?) {
        throw "docker build failed"
    }
} catch {
    Write-Host "ERROR: Docker build failed for DbMigrator" -ForegroundColor Red
    exit 1
}
Write-Host "Docker image built and pushed successfully for DbMigrator (tags: $version, latest)" -ForegroundColor Green


Write-Host "********* BUILDING Blazor Application *********" -ForegroundColor Green
$blazorFolder = Join-Path $slnFolder "src/HC.Blazor"
Set-Location $blazorFolder
Write-Host "Publishing Blazor..." -ForegroundColor Yellow
try {
    $result = dotnet publish -c Release -o bin/Release/net10.0/publish 2>&1
    if (-not $?) {
        throw "dotnet publish failed"
    }
} catch {
    Write-Host "ERROR: dotnet publish failed for Blazor" -ForegroundColor Red
    Write-Host $result -ForegroundColor Red
    exit 1
}
Start-Sleep -Seconds 1  # Wait for file system to sync
$currentDir = Get-Location
$publishPath = Join-Path $currentDir "bin/Release/net10.0/publish"
$publishPathFull = [System.IO.Path]::GetFullPath($publishPath)
if (-not (Test-Path $publishPathFull)) {
    Write-Host "ERROR: Publish folder not found: $publishPathFull" -ForegroundColor Red
    Write-Host "Current directory: $currentDir" -ForegroundColor Yellow
    exit 1
}
Write-Host "Publish successful. Output: $publishPathFull" -ForegroundColor Green
Write-Host "Building Docker image for Blazor (linux/amd64)..." -ForegroundColor Yellow
try {
    docker buildx build --platform linux/amd64 -f Dockerfile.local -t longnguyen1331/hc-blazor:$version -t longnguyen1331/hc-blazor:latest . --push
    if (-not $?) {
        throw "docker build failed"
    }
} catch {
    Write-Host "ERROR: Docker build failed for Blazor" -ForegroundColor Red
    exit 1
}
Write-Host "Docker image built and pushed successfully for Blazor (tags: $version, latest)" -ForegroundColor Green

Write-Host "********* BUILDING Api.Host Application *********" -ForegroundColor Green
$hostFolder = Join-Path $slnFolder "src/HC.HttpApi.Host"
Set-Location $hostFolder
Write-Host "Publishing Api.Host..." -ForegroundColor Yellow
try {
    $result = dotnet publish -c Release -o bin/Release/net10.0/publish 2>&1
    if (-not $?) {
        throw "dotnet publish failed"
    }
} catch {
    Write-Host "ERROR: dotnet publish failed for Api.Host" -ForegroundColor Red
    Write-Host $result -ForegroundColor Red
    exit 1
}
Start-Sleep -Seconds 1  # Wait for file system to sync
$currentDir = Get-Location
$publishPath = Join-Path $currentDir "bin/Release/net10.0/publish"
$publishPathFull = [System.IO.Path]::GetFullPath($publishPath)
if (-not (Test-Path $publishPathFull)) {
    Write-Host "ERROR: Publish folder not found: $publishPathFull" -ForegroundColor Red
    Write-Host "Current directory: $currentDir" -ForegroundColor Yellow
    Write-Host "Checking if bin/Release/net10.0 exists..." -ForegroundColor Yellow
    $binPath = Join-Path $currentDir "bin/Release/net10.0"
    if (Test-Path $binPath) {
        Write-Host "bin/Release/net10.0 exists. Contents:" -ForegroundColor Yellow
        Get-ChildItem $binPath | Select-Object Name, PSIsContainer | Format-Table
    } else {
        Write-Host "bin/Release/net10.0 does not exist" -ForegroundColor Red
    }
    exit 1
}
Write-Host "Publish successful. Output: $publishPathFull" -ForegroundColor Green
Write-Host "Building Docker image for Api.Host (linux/amd64)..." -ForegroundColor Yellow
try {
    docker buildx build --platform linux/amd64 -f Dockerfile.local -t longnguyen1331/hc-api:$version -t longnguyen1331/hc-api:latest . --push
    if (-not $?) {
        throw "docker build failed"
    }
} catch {
    Write-Host "ERROR: Docker build failed for Api.Host" -ForegroundColor Red
    exit 1
}
Write-Host "Docker image built and pushed successfully for Api.Host (tags: $version, latest)" -ForegroundColor Green

Write-Host "********* BUILDING AuthServer Application *********" -ForegroundColor Green
$authServerAppFolder = Join-Path $slnFolder "src/HC.AuthServer"
Set-Location $authServerAppFolder
Write-Host "Publishing AuthServer..." -ForegroundColor Yellow
try {
    $result = dotnet publish -c Release -o bin/Release/net10.0/publish 2>&1
    if (-not $?) {
        throw "dotnet publish failed"
    }
} catch {
    Write-Host "ERROR: dotnet publish failed for AuthServer" -ForegroundColor Red
    Write-Host $result -ForegroundColor Red
    exit 1
}
Start-Sleep -Seconds 2  # Wait for file system to sync
$currentDir = Get-Location
$publishPath = Join-Path $currentDir "bin/Release/net10.0/publish"
$publishPathFull = [System.IO.Path]::GetFullPath($publishPath)
# Retry checking the path a few times
$maxRetries = 5
$retryCount = 0
while (-not (Test-Path $publishPathFull) -and $retryCount -lt $maxRetries) {
    Start-Sleep -Seconds 1
    $retryCount++
}
if (-not (Test-Path $publishPathFull)) {
    Write-Host "ERROR: Publish folder not found: $publishPathFull" -ForegroundColor Red
    Write-Host "Current directory: $currentDir" -ForegroundColor Yellow
    Write-Host "Checking if bin/Release/net10.0 exists..." -ForegroundColor Yellow
    $binPath = Join-Path $currentDir "bin/Release/net10.0"
    if (Test-Path $binPath) {
        Write-Host "bin/Release/net10.0 exists. Contents:" -ForegroundColor Yellow
        Get-ChildItem $binPath | Select-Object Name, PSIsContainer | Format-Table
    }
    exit 1
}
Write-Host "Publish successful. Output: $publishPathFull" -ForegroundColor Green
Write-Host "Building Docker image for AuthServer (linux/amd64)..." -ForegroundColor Yellow
try {
    docker buildx build --platform linux/amd64 -f Dockerfile.local -t longnguyen1331/hc-authserver:$version -t longnguyen1331/hc-authserver:latest . --push
    if (-not $?) {
        throw "docker build failed"
    }
} catch {
    Write-Host "ERROR: Docker build failed for AuthServer" -ForegroundColor Red
    exit 1
}
Write-Host "Docker image built and pushed successfully for AuthServer (tags: $version, latest)" -ForegroundColor Green


### ALL COMPLETED
Write-Host "COMPLETED" -ForegroundColor Green
Set-Location $currentFolder
exit $LASTEXITCODE