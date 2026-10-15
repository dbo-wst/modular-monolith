# Ordering Persistence Integration Tests

Tests the Ordering EF Core persistence adapter (`src/Modules/Ordering/Persistence`) against a real SQL Server: repositories, DBO mappers and the `UnitOfWork`. Assembly: `Acme.Core.Ordering.Persistence.IntegrationTest`.

## Structure

| Item | Kind | Description |
|---|---|---|
| `Orders` | folder | `Order` mapper and repository read/write tests (no own README, see Contents) |
| `OrderItems` | folder | `OrderItem` mapper round-trip tests (no own README, see Contents) |

## Contents

| Item | Kind | Description |
|---|---|---|
| `TestsCollectionDefinition.cs` | collection | Binds collection `IntegrationTestsFixture.CollectionName` to `IntegrationTestsFixture` |
| `UnitOfWorkTests.cs` | test class | `SaveChangesAsync` persists; `ExecuteInTransactionAsync` commits, rolls back, returns work value/error, handles thrown exceptions; cancellation, concurrency conflict and primary key violation mapped to typed errors |
| `Orders/OrderMapperTests.cs` | test class | `OrderStatus` to `EStatusDbo` and back, full round-trip, missing DBO status rehydrates as `New` (no database) |
| `Orders/ReadOnlyOperationsTests.cs` | test class | `GetByIdAsync` for missing and persisted orders |
| `Orders/WriteOnlyOperationsTests.cs` | test class | Create, update, delete orders, including fail cases for existing/missing entities |
| `OrderItems/OrderItemMapperTests.cs` | test class | DBO to domain and back round-trip (no database) |
| `OrderItems/ReadOnlyOperationsTests.cs` | test class | Rehydrated `OrderItem` preserves values (no database) |
| `OrderItems/WriteOnlyOperationsTests.cs` | test class | `OrderItem` DBO round-trips through the mapper (no database) |
| `appsettings.json` | config | Logging/Serilog settings (console and rolling file `logs/log-.log`) |
| `Acme.Core.Ordering.Persistence.IntegrationTest.csproj` | project | Test project definition |

## How it works

Database tests are in `[Collection(IntegrationTestsFixture.CollectionName)]` and resolve `IOrderingDbContextFactory`, `IOrderRepository` and `IUnitOfWork` from `IntegrationTestsFixture` (see [`../TestCompanion`](../TestCompanion/README.md)), or use `OrderingPersistence.Create(factory)` to get repositories and unit of work on one context. Test data comes from `Support.Domain.NewOrder()` / `NewOrderItem()`. Mapper tests are plain facts and do not touch the database.

Prerequisites:

- .NET SDK 10
- Docker Desktop / Docker Engine (default container mode)

If `appsettings.test.json` in this folder is missing or does not contain `Persistence:ConnectionString`, the fixture starts a temporary SQL Server container via Testcontainers. Otherwise it uses that SQL Server. Pending EF Core migrations are applied in both modes.

## Running

Default (SQL Server container), from the repository root:

```powershell
dotnet test .\test_integration\Modules\Ordering\Persistence\Acme.Core.Ordering.Persistence.IntegrationTest.csproj
```

Using a local SQL Server instead of Testcontainers:

1. Start a local SQL Server (optional helper compose):

```powershell
docker compose -f .\operations\sql\docker-compose.mssql.local.yml up -d
```

2. Create the local file `test_integration/Modules/Ordering/Persistence/appsettings.test.json`:

```json
{
  "Persistence": {
    "ConnectionString": "Server=tcp:localhost,1433;Database=OrderingIntegrationTests;User Id=sa;Password=MyP@ssword;TrustServerCertificate=True;"
  }
}
```

3. Run the tests with the command above.

Troubleshooting:

- Docker mode fails: ensure Docker is running.
- SQL connection fails in local mode: verify `Persistence:ConnectionString` in `appsettings.test.json`.
- Configuration changes are not picked up: rebuild so `appsettings.test.json` is copied to the output, then rerun.

## Dependencies

- References: `test_integration/Modules/Ordering/TestCompanion/Acme.Core.Ordering.TestCompanion.csproj` (which brings in Ordering Domain and Persistence)
- Packages: `xunit.v3`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`, `Microsoft.Extensions.Configuration.FileExtensions`, `Microsoft.Extensions.Configuration.Json`, `Serilog`, `Serilog.Extensions.Logging`, `Serilog.Settings.Configuration`

## Extending

For a new aggregate add `<Aggregate>s/` with `<Aggregate>MapperTests.cs`, `ReadOnlyOperationsTests.cs` and `WriteOnlyOperationsTests.cs`; database tests join collection `IntegrationTestsFixture.CollectionName`. New `UnitOfWork` behaviour goes into `UnitOfWorkTests.cs`.
