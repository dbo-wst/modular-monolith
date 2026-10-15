# Module Integration Tests

Integration tests per business module, run against a real SQL Server (Testcontainers by default). Each module folder mirrors `src/Modules/<Module>`.

## Contents

| Item | Kind | Description |
|---|---|---|
| [`Ordering`](Ordering/README.md) | folder | Ordering Persistence and Application integration tests and their test companion |

## How it works

A module's `TestCompanion` provides an xUnit collection fixture that starts a SQL Server container (or uses `Persistence:ConnectionString` from `appsettings.test.json`), registers the module's persistence and applies migrations. Docker is required in default mode.

## Running

```powershell
dotnet test .\test_integration\Modules\Ordering\Persistence\Acme.Core.Ordering.Persistence.IntegrationTest.csproj
dotnet test .\test_integration\Modules\Ordering\Application\Acme.Core.Ordering.Application.IntegrationTest.csproj
```

## Dependencies

- References: `src/Modules/<Module>/*`, `test_integration/TestCompanion`

## Extending

For a new module `<Module>`, create `test_integration/Modules/<Module>/` with `TestCompanion/` (fixture calling `Add<Module>Persistence`), `Persistence/` and `Application/` integration test projects named `Acme.Core.<Module>.<Layer>.IntegrationTest`, each with an `appsettings.json`, and register them in `Acme.Core.slnx`.
