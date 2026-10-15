# Ordering Persistence Integration Tests

## Prerequisites

- .NET SDK 10
- Docker Desktop / Docker Engine (for default container mode)

## Start Tests (Default: Sql Container)

If `test_integration/Modules/Ordering/Persistence/appsettings.test.json` is missing
or does not contain `Persistence:ConnectionString`, tests start a temporary SQL
Server container via Testcontainers.

Run from repository root:

```powershell
dotnet test .\test_integration\Modules\Ordering\Persistence\Template.Core.Ordering.Persistence.IntegrationTest.csproj
```

## Start Tests (Using Local SQL Server Instead of Testcontainers)

1. Start local SQL Server (optional helper compose):

```powershell
docker compose -f .\cicd\sql\docker-compose.mssql.local.yml up -d
```

2. Create local file `test_integration/Modules/Ordering/Persistence/appsettings.test.json`:

```json
{
  "Persistence": {
    "ConnectionString": "Server=tcp:localhost,1433;Database=<db>;User Id=sa;Password=MyP@ssword;TrustServerCertificate=True;"
  }
}
```

3. Run tests:

```powershell
dotnet test .\test_integration\Modules\Ordering\Persistence\Template.Core.Ordering.Persistence.IntegrationTest.csproj
```

## Troubleshooting

- Docker mode fails: ensure Docker is running.
- SQL connection fails in local mode: verify `Persistence:ConnectionString` in `appsettings.test.json`.
- Configuration changes are not picked up: restart IDE/terminal and rerun tests.

