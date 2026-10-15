# Integration Tests

Tests that exercise real infrastructure or a hosted application: the Ordering module against SQL Server, and the two hosts (`Template.Core.Api` gRPC, `Template.Api` HTTP gateway) through `WebApplicationFactory`. This guide is shared for all projects under `test_integration/`.

## Structure

| Item | Kind | Description |
|---|---|---|
| [`Host`](Host/README.md) | folder | In-process host tests for `Template.Api` and `Template.Core.Api` (no database) |
| [`Modules`](Modules/README.md) | folder | SQL Server-backed tests per business module (currently Ordering) |
| [`TestCompanion`](TestCompanion/README.md) | folder (library) | Shared helpers, including the Testcontainers SQL Server container |
| `Directory.Build.props` | build props | net10.0, nullable, `xUnit1051` suppressed; copies `appsettings.json` and an optional `appsettings.test.json` to the output |

## Prerequisites

- .NET SDK 10
- Docker Desktop / Docker Engine for the SQL-backed Ordering projects in default mode (they start a SQL Server container). The `Host` projects do not need Docker.

## Run Integration Tests

Run from the repository root (PowerShell):

```powershell
dotnet test .\test_integration\Modules\Ordering\Persistence\Template.Core.Ordering.Persistence.IntegrationTest.csproj
dotnet test .\test_integration\Modules\Ordering\Application\Template.Core.Ordering.Application.IntegrationTest.csproj
dotnet test .\test_integration\Host\Template.Core.Api.IntegrationTest\Template.Core.Api.IntegrationTest.csproj
dotnet test .\test_integration\Host\Template.Api.IntegrationTest\Template.Api.IntegrationTest.csproj
```

## Configuration

Common behavior:

- Each project has an `appsettings.json` that is copied to the output directory.
- A project may have a local, optional `appsettings.test.json` (not committed) for overrides; it is copied to the output when present.

SQL-backed fixtures (`IntegrationTestsFixture` used by `Modules/Ordering/Persistence` and `Modules/Ordering/Application`):

- If `Persistence:ConnectionString` is missing in `appsettings.test.json`, tests start an ephemeral SQL Server container (`mcr.microsoft.com/mssql/server:2025-latest`) via Testcontainers.
- If `Persistence:ConnectionString` is set, tests use that SQL Server and do not start a container.
- In both cases pending EF Core migrations are applied when `OrderingDbContextFactory` is created.

Example `appsettings.test.json` for local SQL (the password matches `operations/sql/docker-compose.mssql.local.yml`):

```json
{
  "Persistence": {
    "ConnectionString": "Server=tcp:localhost,1433;Database=OrderingIntegrationTests;User Id=sa;Password=MyP@ssword;TrustServerCertificate=True;"
  }
}
```

A local SQL Server can be started with:

```powershell
docker compose -f .\operations\sql\docker-compose.mssql.local.yml up -d
```

## Troubleshooting

- Docker-related failure: ensure Docker is running.
- SQL connection failure in local mode: verify `Persistence:ConnectionString`.
- Config changes not applied: rebuild so `appsettings.test.json` is copied to the output, then rerun tests.
