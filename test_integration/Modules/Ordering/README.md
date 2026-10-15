# Ordering Module Integration Tests

Integration tests for the Ordering module against SQL Server: EF Core persistence (repositories, mappers, unit of work) and the application handlers on top of it.

## Contents

| Item | Kind | Description |
|---|---|---|
| [`Persistence`](Persistence/readme.md) | folder (test project) | Repositories, DBO mappers and `UnitOfWork` (transactions, concurrency, error mapping) on SQL Server |
| [`Application`](Application/README.md) | folder (test project) | Command/query handlers backed by SQL Server |
| [`TestCompanion`](TestCompanion/README.md) | folder (library) | `IntegrationTestsFixture`, `OrderingPersistence`, `RecordingDomainEventDispatcher`, domain object builders |

## How it works

Both test projects define the xUnit collection `IntegrationTestsFixture.CollectionName` with `IntegrationTestsFixture` as collection fixture. The fixture starts one SQL Server container per test project run (or uses `Persistence:ConnectionString` from a local `appsettings.test.json`), calls `AddMediator()` and `AddOrderingPersistence(configuration)`, and EF Core migrations are applied when the `IOrderingDbContextFactory` is resolved. Docker is required in default mode.

## Running

```powershell
dotnet test .\test_integration\Modules\Ordering\Persistence\Template.Core.Ordering.Persistence.IntegrationTest.csproj
dotnet test .\test_integration\Modules\Ordering\Application\Template.Core.Ordering.Application.IntegrationTest.csproj
```

## Dependencies

- References: `src/Modules/Ordering/Domain`, `src/Modules/Ordering/Application`, `src/Modules/Ordering/Persistence`, `test_integration/TestCompanion`

## Extending

New aggregate: add `Persistence/<Aggregate>s/` (mapper, read and write operation tests) and `Application/<Aggregate>s/<Slice>/` handler tests, plus a `TestCompanion/<Aggregate>/Support.Domain.cs` builder.
