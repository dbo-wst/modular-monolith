# Ordering Persistence

EF Core / SQL Server adapter of the Ordering module (assembly `Acme.Core.Ordering.Persistence`). It implements the domain repository ports, the application read-store port and the unit of work, and owns the `ordering` database schema and its migrations.

## Contents

| Item | Kind | Description |
|---|---|---|
| [`DbContext/`](DbContext/README.md) | folder | `OrderingDbContext`, runtime factory (auto-migrate), design-time factory. |
| [`Orders/`](Orders/README.md) | folder | `OrderDbo`, `EStatusDbo`, EF configuration, `OrderRepository`, mappers. |
| [`OrderItems/`](OrderItems/README.md) | folder | `OrderItemDbo`, EF configuration, `OrderItemRepository`, mapper. |
| `Migrations/` | folder | EF Core migrations (generated; see [Migrations](#migrations)). |
| `ServiceCollectionExtensions.cs` | static class | `AddOrderingPersistence(services, configuration)` registers everything below. |
| `UnitOfWork.cs` | sealed class | `UnitOfWork<OrderingDbContext>` from BuildingBlocks: save + domain event dispatch. |
| `ReadStore.cs` | sealed class | `IOrderingReadStore`: no-tracking reads projected to Contracts DTOs, with paging. |
| `IOrderingDbContextFactory.cs` | interface | Creates `OrderingDbContext` instances (sync/async). |
| `DboMappers.cs` | static class | Singleton mapper instances `DboMappers.Order`, `DboMappers.OrderItem`. |
| `Paging.cs` | internal static class | `Normalize(pageNumber, pageSize)` -> `(Skip, Take)`. |
| `EnumUtil.cs` | static class | `HasEnumConversion` / `HasNullableEnumConversion`: store enums as their `[EnumMember]` string. |
| `PersistenceOptions.cs` | class | `ConnectionString`, bound from configuration section `Persistence`. |
| `PersistenceOptionsValidator.cs` | sealed class | FluentValidation: `ConnectionString` not empty. |
| `Constants.cs` | static class | Schema `ordering`, migrations table `__EFMigrationsHistory`, `SQL_CONNECTION_STRING` env lookup (design time). |
| `AssemblyReference.cs` | static class + `Anchor` record | Assembly handle used to apply all `IEntityTypeConfiguration`s. |
| `Acme.Core.Ordering.Persistence.csproj` | project | EF Core SQL Server adapter project. |

## How it works

Registration (`AddOrderingPersistence`):

- `PersistenceOptions` bound from `Persistence`, validated with FluentValidation on start.
- Pooled `IDbContextFactory<OrderingDbContext>` on SQL Server with `EnableRetryOnFailure` and the migrations history table in schema `ordering`.
- `IOrderingDbContextFactory` (singleton); its constructor applies pending migrations once if the provider is relational.
- Scoped: one `OrderingDbContext` per scope (shared by unit of work and repositories), `DomainEventCollector` (`TryAdd`), `IUnitOfWork`, `IOrderRepository`, `IOrderItemRepository`, `IOrderingReadStore`.
- `IDomainEventDispatcher` is not registered here; the host provides it via `AddMediator()`.

Write side:

1. Repositories map aggregates to Dbos (`DboMappers`), so EF never tracks aggregates. `AddAsync` fails with `ConflictError` if the id exists; `UpdateAsync`/`DeleteAsync` fail with `NotFoundError` if the row is missing; `GetByIdAsync` reads with `AsNoTracking` and rehydrates the aggregate.
2. Every add/update/delete calls `DomainEventCollector.Track(aggregate)`.
3. `UnitOfWork.SaveChangesAsync` saves; on success it drains the collector and dispatches the events, on failure it returns `OperationCancelled`, `ConcurrencyConflict` or `PersistenceFailed` and dispatches nothing.

Read side: `ReadStore` creates its own short-lived context from `IOrderingDbContextFactory`, reads with `AsNoTracking`, applies `Paging.Normalize` (page clamped to 1..1000, size <= 0 -> 10, max 100) and maps rows to DTOs. Single reads return `NotFoundError` when missing.

## Dependencies

- References: `Acme.Core.Ordering.Domain`, `Acme.Core.Ordering.Application`, `Acme.Core.BuildingBlocks.Persistence`; packages `LanguageExt.Core`, `Microsoft.EntityFrameworkCore`, `.SqlServer`, `.Design` (private), `Microsoft.Extensions.Options`, `.Options.ConfigurationExtensions`, `Microsoft.Extensions.Configuration.Abstractions`.
- Used by: `src/Host/Acme.Core.Api` (`AddOrderingModule` -> `AddOrderingPersistence`), `test_integration/Modules/Ordering/Persistence`, `test_integration/Modules/Ordering/TestCompanion`, `test/Modules/Ordering/TestCompanion`, `test/Architecture` (listed in `Adapters`).

## Extending

- New aggregate: add a folder with `XxxDbo`, `Configuration : IEntityTypeConfiguration<XxxDbo>` (picked up automatically), `Mappers/XxxMapper : IMap<Domain, Dbo>` exposed in `DboMappers`, and `XxxRepository` that tracks aggregates in `DomainEventCollector`; add a `DbSet` to `OrderingDbContext`, register the repository in `AddOrderingPersistence`, and add a migration.
- New read model: extend `IOrderingReadStore` in Application and implement it in `ReadStore`.

## Migrations

### Set connection string

PowerShell:

```powershell
[Environment]::SetEnvironmentVariable("SQL_CONNECTION_STRING", "Server=tcp:localhost,1433;Database=acme-local;User Id=sa;Password=MyP@ssword;TrustServerCertificate=True;")
```

Bash (Linux/macOS):

```bash
export SQL_CONNECTION_STRING="Server=tcp:localhost,1433;Database=acme-local;User Id=sa;Password=MyP@ssword;TrustServerCertificate=True;"
```

### Get connection string

PowerShell:

```powershell
[Environment]::GetEnvironmentVariable("SQL_CONNECTION_STRING")
```

Bash (Linux/macOS):

```bash
echo "$SQL_CONNECTION_STRING"
```

### Add a migration

Run from repository root.

PowerShell:

```powershell
dotnet ef migrations add AddOrderingModule `
  --project .\src\Modules\Ordering\Persistence\Acme.Core.Ordering.Persistence.csproj `
  --context Acme.Core.Ordering.Persistence.DbContext.OrderingDbContext
```

Bash (Linux/macOS):

```bash
dotnet ef migrations add AddOrderingModule \
  --project ./src/Modules/Ordering/Persistence/Acme.Core.Ordering.Persistence.csproj \
  --context Acme.Core.Ordering.Persistence.DbContext.OrderingDbContext
```

Migration files are generated in `src/Modules/Ordering/Persistence/Migrations`.

### Apply migrations to database

PowerShell:

```powershell
dotnet ef database update `
  --project .\src\Modules\Ordering\Persistence\Acme.Core.Ordering.Persistence.csproj `
  --context Acme.Core.Ordering.Persistence.DbContext.OrderingDbContext
```

Bash (Linux/macOS):

```bash
dotnet ef database update \
  --project ./src/Modules/Ordering/Persistence/Acme.Core.Ordering.Persistence.csproj \
  --context Acme.Core.Ordering.Persistence.DbContext.OrderingDbContext
```
