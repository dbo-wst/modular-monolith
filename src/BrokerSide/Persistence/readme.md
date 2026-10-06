# BrokerSide Persistence: Migrations

## Set connection string

PowerShell:

```powershell
[Environment]::SetEnvironmentVariable("SQL_CONNECTION_STRING", "Server=tcp:localhost,1433;Database=wst-trios-core-local;User Id=sa;Password=MyP@ssword;TrustServerCertificate=True;")
```

Bash (Linux/macOS):

```bash
export SQL_CONNECTION_STRING="Server=tcp:localhost,1433;Database=wst-trios-core-local;User Id=sa;Password=MyP@ssword;TrustServerCertificate=True;"
```

## Get connection string

PowerShell:

```powershell
[Environment]::GetEnvironmentVariable("SQL_CONNECTION_STRING")
```

Bash (Linux/macOS):

```bash
echo "$SQL_CONNECTION_STRING"
```

## Add a migration

Run from repository root.

PowerShell:

```powershell
dotnet ef migrations add AddBrokerSideModule `
  --project .\src\BrokerSide\Persistence\Wst.Trios.Core.BrokerSide.Persistence.csproj `
  --context Wst.Trios.Core.BrokerSide.Persistence.DbContext.BrokerSideDbContext
```

Bash (Linux/macOS):

```bash
dotnet ef migrations add AddBrokerSideModule \
  --project ./src/BrokerSide/Persistence/Wst.Trios.Core.BrokerSide.Persistence.csproj \
  --context Wst.Trios.Core.BrokerSide.Persistence.DbContext.BrokerSideDbContext
```

Migration files are generated in `src/BrokerSide/Persistence/Migrations`.

## Apply migrations to database

PowerShell:

```powershell
dotnet ef database update `
  --project .\src\BrokerSide\Persistence\Wst.Trios.Core.BrokerSide.Persistence.csproj `
  --context Wst.Trios.Core.BrokerSide.Persistence.DbContext.BrokerSideDbContext
```

Bash (Linux/macOS):

```bash
dotnet ef database update \
  --project ./src/BrokerSide/Persistence/Wst.Trios.Core.BrokerSide.Persistence.csproj \
  --context Wst.Trios.Core.BrokerSide.Persistence.DbContext.BrokerSideDbContext
```
