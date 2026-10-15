# Order Item Mappers (Persistence)

Mapper between the `OrderItem` aggregate and its database model (EF Core adapter).

## Contents

| Item | Kind | Description |
|---|---|---|
| `OrderItemMapper.cs` | struct, `IMap<Domain.OrderItems.OrderItem, OrderItemDbo>` | Dbo -> aggregate via `OrderItem.Rehydrate`; aggregate -> new `OrderItemDbo`. |

## How it works

- Dbo -> domain: nullable columns become nullable value objects (`OrderId.From`, `Quantity.From`, `Price.From`); `OrderItem.Rehydrate` fills defaults for missing values and raises no events.
- Domain -> Dbo: unwraps value objects; `RowVersion` is left unset.
- Exposed as `DboMappers.OrderItem`. `ReadStore` does not use it; it projects `OrderItemDbo` to the DTO directly.

## Dependencies

- References: `BuildingBlocks.Domain` (`IMap`), Domain `OrderItems`, `Orders`, `ValueObjects`, `../OrderItemDbo`.
- Used by: `OrderItemRepository` (via `DboMappers.OrderItem`), mapper integration tests (`test_integration/Modules/Ordering/Persistence/OrderItems/OrderItemMapperTests.cs`).

## Extending

Keep both `Map` overloads in sync with every field added to `OrderItemDbo` or the aggregate.
