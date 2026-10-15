# Orders (Persistence)

Persistence of the `Order` aggregate: database model, EF configuration, repository and mappers (EF Core adapter).

## Contents

| Item | Kind | Description |
|---|---|---|
| `OrderDbo.cs` | class | Row model: `OrderId`, `RowVersion`, `Number?`, `EffectiveDate?`, `Status?` (`EStatusDbo`). |
| `EStatusDbo.cs` | enum | Stored status with `[EnumMember]` values `NEW`, `IN_PROGRESS`, `COMPLETED`, `CANCELLED`, `REJECTED`. |
| `Configuration.cs` | class : `IEntityTypeConfiguration<OrderDbo>` | Table `Orders`, key `OrderId`, `RowVersion` as row version, `Number` max 128, `Status` as string. |
| `OrderRepository.cs` | class : `IOrderRepository` | Add/Update/Delete/GetById on `OrderingDbContext`, tracks aggregates for event dispatch. |
| [`Mappers/`](Mappers/README.md) | folder | `OrderMapper` (domain <-> Dbo), `EStatusDboMapper`. |

## How it works

- Table `ordering.Orders`; `Status` is stored as the `[EnumMember]` string via `HasNullableEnumConversion` (e.g. `IN_PROGRESS`).
- `OrderRepository`:
  - `AddAsync`: `ConflictError.AlreadyExists` if the id exists, otherwise adds the mapped Dbo.
  - `UpdateAsync`: loads the tracked row (`NotFoundError` if missing) and copies values with `Entry(found).CurrentValues.SetValues(mapped)`.
  - `DeleteAsync`: loads the row (`NotFoundError` if missing) and removes it.
  - `GetByIdAsync`: `AsNoTracking` read, rehydrated via `DboMappers.Order`; `NotFoundError` if missing.
  - Add/Update/Delete call `DomainEventCollector.Track(order)`; nothing is saved until `IUnitOfWork.SaveChangesAsync`.

## Dependencies

- References: Domain `Orders`, `ValueObjects`; `BuildingBlocks.Domain.Errors`, `BuildingBlocks.Persistence` (`DomainEventCollector`); `../DbContext`, `../DboMappers`, `../EnumUtil`.
- Used by: `OrderingDbContext` (`DbSet<OrderDbo>`), `AddOrderingPersistence` (registers `OrderRepository` as `IOrderRepository`), `ReadStore` (reads `OrderDbo`), migrations, integration tests (`test_integration/Modules/Ordering/Persistence/Orders`).

## Extending

For a new order property: add it to `OrderDbo`, configure it in `Configuration`, map it in `Mappers/OrderMapper` and `ReadStore.ToDto`, then add a migration.
