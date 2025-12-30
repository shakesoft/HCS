./build-image.ps1 -ProjectPath "../../src/HC.DbMigrator/HC.DbMigrator.csproj" -ImageName hc/dbmigrator
./build-image.ps1 -ProjectPath "../../src/HC.HttpApi.Host/HC.HttpApi.Host.csproj" -ImageName hc/httpapihost
./build-image.ps1 -ProjectPath "../../src/HC.Blazor/HC.Blazor.csproj" -ImageName hc/blazorserver 
./build-image.ps1 -ProjectPath "../../src/HC.AuthServer/HC.AuthServer.csproj" -ImageName hc/authserver
