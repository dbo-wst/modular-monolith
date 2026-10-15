# DbContext

EF Core context and its factories for the Ordering persistence adapter.

## Contents

| Item | Kind | Description |
|---|---|---|
| `OrderingDbContext.cs` | class : `DbContext` | `DbSet<OrderDbo> Orders`, `DbSet<OrderItemDbo> OrderItems`; default schema `ordering`; applies all configurations from this assembly. |
| `OrderingDbContextFactory.cs` | class : `IOrderingDbContextFactory` | Wraps the pooled `IDbContextFactory<OrderingDbContext>`; applies pending migrations in its constructor. |
| `OrderingDbDesignTimeContextFactory.cs` | class : `IDesignTimeDbContextFactory<OrderingDbContext>` | Used by `dotnet ef`; SQL Server with connection string from `SQL_CONNECTION_STRING`. |

## How it works

- `OnModelCreating`: `HasDefaultSchema(Constants.Schema)` + `ApplyConfigurationsFromAssembly(AssemblyReference.Assembly)`, so every `Configuration` class in `Orders/` and `OrderItems/` is picked up.
- `OrderingDbContextFactory` is registered as a singleton. On first resolution it creates a context and, if the provider is relational and migrations are pending, calls `Database.Migrate()` (inside an `Activity` named `OrderingDbContextFactory.Migrate` when tracing is active).
- The scoped `OrderingDbContext` used by repositories and the unit of work is created via `IOrderingDbContextFactory.CreateDbContext()`; `ReadStore` creates its own contexts via `CreateDbContextAsync`.
- Both runtime and design-time setups use migrations history table `__EFMigrationsHistory` in schema `ordering`.

## Dependencies

- References: `Microsoft.EntityFrameworkCore`, `.SqlServer`, `.Design`; `../Constants`, `../AssemblyReference`, `../Orders`, `../OrderItems`.
- Used by: `ServiceCollectionExtensions.AddOrderingPersistence`, `UnitOfWork`, `OrderRepository`, `OrderItemRepository`, `ReadStore`, `Migrations/`, `dotnet ef` tooling, integration tests.

## Extending

Add a `DbSet<XxxDbo>` property for each new table; its `IEntityTypeConfiguration` is applied automatically. Then add a migration (see [Migrations](../readme.md#migrations)).
