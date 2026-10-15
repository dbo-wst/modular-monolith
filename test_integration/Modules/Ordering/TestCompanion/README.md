# Ordering Test Companion (integration)

Ordering-specific helpers for the SQL Server-backed integration test projects: the collection fixture, a scope-like persistence bundle, a recording dispatcher and domain object builders. Assembly: `Template.Core.Ordering.TestCompanion` (library, contains no tests).

## Contents

| Item | Kind | Description |
|---|---|---|
| `IntegrationTestsFixture.cs` | fixture | `IAsyncLifetime` collection fixture: starts a SQL Server container or uses `appsettings.test.json`, configures Serilog, registers `AddMediator()` and `AddOrderingPersistence(...)`; exposes `GetRequiredService<T>()`, `Faker`, `CollectionName` |
| `OrderingPersistence.cs` | helper | `OrderingPersistence.Create(factory, dispatcher?)`: `UnitOfWork`, `Orders`, `OrderItems` sharing one `OrderingDbContext` and `DomainEventCollector`; `ForOrders`/`ForOrderItems` build handlers |
| `RecordingDomainEventDispatcher.cs` | fake | `IDomainEventDispatcher` recording events in `Dispatched`; fails every dispatch when given an `Error` |
| `Order/Support.Domain.cs` | helper | `Support.Domain.NewOrder()`: rehydrated `Order` with a new id, `ORD-1`, 2026-07-10, `New` |
| `OrderItem/Support.Domain.cs` | helper | `Support.Domain.NewOrderItem()`: rehydrated `OrderItem` with new ids, quantity 100.5, price 25.75 |
| `Template.Core.Ordering.TestCompanion.csproj` | project | Library project definition |

## How it works

`IntegrationTestsFixture.InitializeAsync` reads `Persistence:ConnectionString` from an optional `appsettings.test.json` in the working directory:

- empty: `Template.Core.TestCompanion.Support.Infrastructure.UseContainer()` starts a SQL Server container and its connection string is passed as in-memory configuration;
- set: configuration is built from `appsettings.json` + `appsettings.test.json`.

Pending migrations are applied by `OrderingDbContextFactory` on construction. `DisposeAsync` disposes the container and the service provider and flushes Serilog. This assembly has the same name as `test/Modules/Ordering/TestCompanion` but uses SQL Server instead of the in-memory provider.

## Running

Not run directly; consumed by `test_integration/Modules/Ordering/Persistence` and `test_integration/Modules/Ordering/Application`. Docker is needed at runtime in container mode.

## Dependencies

- References: `src/Modules/Ordering/Domain`, `src/Modules/Ordering/Persistence`, `test_integration/TestCompanion/Template.Core.TestCompanion.csproj`
- Packages: `Bogus`, `FakeItEasy`, `LanguageExt.Core`, `Microsoft.EntityFrameworkCore.InMemory`, `Microsoft.Extensions.Configuration(.Json, .EnvironmentVariables)`, `Microsoft.Extensions.DependencyInjection(.Abstractions)`, `Microsoft.Extensions.Options`, `Testcontainers.MsSql`, `Selenium.WebDriver`, `Serilog`, `Serilog.Extensions.Logging`, `Serilog.Settings.Configuration`, `coverlet.collector`, `xunit.v3`

## Extending

New aggregate: add `<Aggregate>/Support.Domain.cs` with a `New<Aggregate>()` builder and, if it has a repository, add it to `OrderingPersistence`. Services needed by tests must be registered in `IntegrationTestsFixture.BuildServiceProvider`.
