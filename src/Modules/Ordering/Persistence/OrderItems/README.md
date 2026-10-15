# Order Items (Persistence)

Persistence of the `OrderItem` aggregate: database model, EF configuration, repository and mapper (EF Core adapter).

## Contents

| Item | Kind | Description |
|---|---|---|
| `OrderItemDbo.cs` | class | Row model: `OrderItemId`, `RowVersion?`, `OrderId?`, `Quantity?`, `Price?`. |
| `Configuration.cs` | class : `IEntityTypeConfiguration<OrderItemDbo>` | Table `OrderItems`, key `OrderItemId`, `RowVersion` as row version, `Quantity`/`Price` precision (18, 2). |
| `OrderItemRepository.cs` | class : `IOrderItemRepository` | Add/Update/Delete/GetById on `OrderingDbContext`, tracks aggregates for event dispatch. |
| [`Mappers/`](Mappers/README.md) | folder | `OrderItemMapper` (domain <-> Dbo). |

## How it works

- Table `ordering.OrderItems`. `OrderId` is a plain nullable column: no foreign key or relationship to `Orders` is configured.
- `OrderItemRepository` behaves like `OrderRepository`:
  - `AddAsync`: `ConflictError.AlreadyExists` if the id exists.
  - `UpdateAsync`: loads the tracked row (`NotFoundError` if missing) and copies values via `SetValues`.
  - `DeleteAsync`: loads and removes the row (`NotFoundError` if missing).
  - `GetByIdAsync`: `AsNoTracking` read, rehydrated via `DboMappers.OrderItem`.
  - Add/Update/Delete call `DomainEventCollector.Track(orderItem)`; changes are saved by the unit of work.

## Dependencies

- References: Domain `OrderItems`, `Orders`, `ValueObjects`; `BuildingBlocks.Domain.Errors`, `BuildingBlocks.Persistence`; `../DbContext`, `../DboMappers`.
- Used by: `OrderingDbContext` (`DbSet<OrderItemDbo>`), `AddOrderingPersistence` (registers `OrderItemRepository` as `IOrderItemRepository`), `ReadStore`, migrations, integration tests (`test_integration/Modules/Ordering/Persistence/OrderItems`).

## Extending

For a new item property: add it to `OrderItemDbo`, configure it in `Configuration`, map it in `Mappers/OrderItemMapper` and `ReadStore.ToDto`, then add a migration.
