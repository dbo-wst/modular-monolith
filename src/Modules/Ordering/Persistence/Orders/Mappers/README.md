# Order Mappers (Persistence)

Mappers between the `Order` aggregate and its database model (EF Core adapter).

## Contents

| Item | Kind | Description |
|---|---|---|
| `OrderMapper.cs` | struct, `IMap<Domain.Orders.Order, OrderDbo>` | Dbo -> aggregate via `Order.Rehydrate`; aggregate -> new `OrderDbo`. |
| `EStatusDboMapper.cs` | readonly struct, `IMap<OrderStatus, EStatusDbo>` | Two-way value-by-value status mapping; throws `ArgumentOutOfRangeException` for unknown values. |

## How it works

- Dbo -> domain: nullable columns become nullable value objects (`OrderNumber.From`, `EffectiveDate.From`, mapped status or null); `Order.Rehydrate` substitutes defaults for missing values and raises no events.
- Domain -> Dbo: unwraps value objects; `RowVersion` is left at its default.
- `OrderMapper` is exposed as `DboMappers.Order`; `EStatusDboMapper` is used internally by `OrderMapper`.
- `ReadStore` does not use these mappers; it projects `OrderDbo` to the DTO itself.

## Dependencies

- References: `BuildingBlocks.Domain` (`IMap`), Domain `Orders`, `ValueObjects`, `../OrderDbo`, `../EStatusDbo`.
- Used by: `OrderRepository` (via `DboMappers.Order`), mapper integration tests (`test_integration/Modules/Ordering/Persistence/Orders/OrderMapperTests.cs`).

## Extending

Keep mappings symmetric: every new `OrderDbo`/aggregate field must be handled in both `Map` overloads.
