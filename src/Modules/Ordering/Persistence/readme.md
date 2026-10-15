# Ordering Persistence: Migrations

## Set connection string

PowerShell:

```powershell
[Environment]::SetEnvironmentVariable("SQL_CONNECTION_STRING", "Server=tcp:localhost,1433;Database=template-core-local;User Id=sa;Password=MyP@ssword;TrustServerCertificate=True;")
```

Bash (Linux/macOS):

```bash
export SQL_CONNECTION_STRING="Server=tcp:localhost,1433;Database=template-core-local;User Id=sa;Password=MyP@ssword;TrustServerCertificate=True;"
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
dotnet ef migrations add AddOrderingModule `
  --project .\src\Modules\Ordering\Persistence\Template.Core.Ordering.Persistence.csproj `
  --context Template.Core.Ordering.Persistence.DbContext.OrderingDbContext
```

Bash (Linux/macOS):

```bash
dotnet ef migrations add AddOrderingModule \
  --project ./src/Modules/Ordering/Persistence/Template.Core.Ordering.Persistence.csproj \
  --context Template.Core.Ordering.Persistence.DbContext.OrderingDbContext
```

Migration files are generated in `src/Modules/Ordering/Persistence/Migrations`.

## Apply migrations to database

PowerShell:

```powershell
dotnet ef database update `
  --project .\src\Modules\Ordering\Persistence\Template.Core.Ordering.Persistence.csproj `
  --context Template.Core.Ordering.Persistence.DbContext.OrderingDbContext
```

Bash (Linux/macOS):

```bash
dotnet ef database update \
  --project ./src/Modules/Ordering/Persistence/Template.Core.Ordering.Persistence.csproj \
  --context Template.Core.Ordering.Persistence.DbContext.OrderingDbContext
```
