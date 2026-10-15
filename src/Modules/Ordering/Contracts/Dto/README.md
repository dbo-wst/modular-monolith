# DTOs

Data transfer objects exposed by the Ordering module (Contracts layer). They are flat primitive records used in results and as read models.

## Contents

| Item | Kind | Description |
|---|---|---|
| `Order.cs` | sealed record | `(Guid OrderId, string? OrderNumber, DateOnly? EffectiveDate, EStatus? Status)`. |
| `OrderItem.cs` | sealed record | `(Guid OrderItemId, Guid? OrderId, decimal? Quantity, decimal? Price)`. |
| `EStatus.cs` | enum | `New`, `InProgress`, `Completed`, `Cancelled`, `Rejected` (mirrors domain `OrderStatus`). |

## How it works

- Fields are nullable because the read store projects database columns directly (all non-key columns are nullable).
- Produced in two ways: from aggregates by `Application/*/Mappers` (command results) and from database rows by `Persistence/ReadStore` (query results).
- `EStatus` <-> `OrderStatus` mapping is done by `Application/Orders/Mappers/StatusMapper`; the read store converts `EStatusDbo` to `EStatus` by enum name.

## Dependencies

- References: none.
- Used by: `../Results`, `../Commands` (`EStatus`), Application mappers and `IOrderingReadStore`, Persistence `ReadStore`, `Template.Core.Api` gRPC mappers, tests.

## Extending

When adding a field to an aggregate, add it here as a nullable property and update both producers (Application mapper and `ReadStore.ToDto`). New `EStatus` values must be added to all status mappers.
