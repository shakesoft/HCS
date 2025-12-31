param ($version='latest')

$currentFolder = $PSScriptRoot
$slnFolder = Join-Path $currentFolder "../../"

Write-Host "********* BUILDING DbMigrator *********" -ForegroundColor Green
$dbMigratorFolder = Join-Path $slnFolder "src/HC.DbMigrator"
Set-Location $dbMigratorFolder
dotnet publish -c Release
docker buildx build --platform linux/amd64 -f Dockerfile.local -t hc-db-migrator:$version . --push


Write-Host "********* BUILDING Blazor Application *********" -ForegroundColor Green
$blazorFolder = Join-Path $slnFolder "src/HC.Blazor"
Set-Location $blazorFolder
dotnet publish -c Release
docker buildx build --platform linux/amd64 -f Dockerfile.local -t hc-blazor:$version . --push

Write-Host "********* BUILDING Api.Host Application *********" -ForegroundColor Green
$hostFolder = Join-Path $slnFolder "src/HC.HttpApi.Host"
Set-Location $hostFolder
dotnet publish -c Release
docker buildx build --platform linux/amd64 -f Dockerfile.local -t hc-api:$version . --push

Write-Host "********* BUILDING AuthServer Application *********" -ForegroundColor Green
$authServerAppFolder = Join-Path $slnFolder "src/HC.AuthServer"
Set-Location $authServerAppFolder
dotnet publish -c Release
docker buildx build --platform linux/amd64 -f Dockerfile.local -t hc-authserver:$version . --push   


docker push longnguyen1331/hc-db-migrator:$version
docker push longnguyen133/hc-blazor:$version
docker push longnguyen133/hc-api:$version
docker push longnguyen133/hc-authserver:$version


### ALL COMPLETED
Write-Host "COMPLETED" -ForegroundColor Green
Set-Location $currentFolder
exit $LASTEXITCODE