# Ordering Test Companion (unit)

Ordering-specific test helpers for the unit test projects: an in-memory `IOrderingDbContextFactory`, a class fixture, a scope-like persistence bundle and a recording domain event dispatcher. Assembly: `Acme.Core.Ordering.TestCompanion` (library, contains no tests).

## Contents

| Item | Kind | Description |
|---|---|---|
| `Support.Infrastructure.cs` | helper | `Support.Infrastructure.DbContextFactory()`: FakeItEasy `IOrderingDbContextFactory` returning `OrderingDbContext` on a uniquely named EF Core in-memory database |
| `InMemoryFixture.cs` | fixture | xUnit class fixture exposing a `Bogus.Faker` and a `ContextFactory` from `DbContextFactory()` |
| `OrderingPersistence.cs` | helper | `OrderingPersistence.Create(factory, dispatcher?)`: `UnitOfWork`, `Orders` and `OrderItems` repositories sharing one `OrderingDbContext` and one `DomainEventCollector`; `ForOrders`/`ForOrderItems` build handlers |
| `RecordingDomainEventDispatcher.cs` | fake | `IDomainEventDispatcher` that records events in `Dispatched`; constructed with an `Error` it fails every dispatch |
| `Acme.Core.Ordering.TestCompanion.csproj` | project | Library project definition |

## How it works

Each call to `DbContextFactory()` uses a new GUID as database name, so every factory is an isolated store; contexts created from the same factory share data. `OrderingPersistence` defaults to a succeeding `RecordingDomainEventDispatcher` and must be disposed (`await using`). `test_integration/Modules/Ordering/TestCompanion` has a same-named assembly with SQL Server-backed equivalents.

## Running

Not run directly; consumed by `test/Modules/Ordering/Domain` and `test/Modules/Ordering/Application`.

## Dependencies

- References: `src/Modules/Ordering/Domain`, `src/Modules/Ordering/Persistence`, `test/TestCompanion/Acme.Core.TestCompanion.csproj`
- Packages: `Bogus`, `FakeItEasy`, `LanguageExt.Core`, `Microsoft.EntityFrameworkCore.InMemory`, `Microsoft.Extensions.Configuration(.Json)`, `Microsoft.Extensions.DependencyInjection(.Abstractions)`, `Microsoft.Extensions.Options`, `Testcontainers.MsSql`, `Selenium.WebDriver`, `Serilog`, `Serilog.Extensions.Logging`, `Serilog.Settings.Configuration`, `coverlet.collector`, `xunit.v3`

## Extending

When a new repository is added to the module, add it to `OrderingPersistence` (constructed with the same context and collector) together with a `For<Aggregate>s` builder. A new module gets its own `test/Modules/<Module>/TestCompanion` following this layout.
