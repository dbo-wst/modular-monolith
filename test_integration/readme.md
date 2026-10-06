# Integration Tests

This guide is shared for all projects under `test_integration/`.

## Prerequisites

- .NET SDK 10
- Docker Desktop / Docker Engine (required for tests that start SQL containers)

## Run Integration Tests

Run one integration test project:

```powershell
dotnet test .\test_integration\<Domain>\<Module>\*.IntegrationTest.csproj
```


## Configuration

Common behavior:

- Projects load `appsettings.json`.
- Projects can use optional `appsettings.test.json` for local overrides.

SQL-backed fixtures (for example `Issuance/ShareApplication`) behavior:

- If `Persistence:ConnectionString` is missing in `appsettings.test.json`, tests start an ephemeral SQL Server container via Testcontainers.
- If `Persistence:ConnectionString` is set, tests use that SQL Server and do not start a container.

Example `appsettings.test.json` for local SQL:

```json
{
  "Persistence": {
    "ConnectionString": "Server=tcp:localhost,1433;Database=--db-local;User Id=sa;Password=MyP@ssword;TrustServerCertificate=True;"
  }
}
```

## Troubleshooting

- Docker-related failure: ensure Docker is running.
- SQL connection failure in local mode: verify `Persistence:ConnectionString`.
- Config changes not applied: restart terminal/IDE and rerun tests.
