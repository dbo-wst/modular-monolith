# Results

Success payloads of the Ordering commands and queries (Contracts layer). Each request type has exactly one result record; handlers return it wrapped in `Fin<TResult>`.

## Contents

| Item | Kind | Description |
|---|---|---|
| `CreateOrderResult.cs` | sealed record | `(Order Order)` - the created order. |
| `UpdateOrderResult.cs` | sealed record | `(Order Order)` - the order after the update. |
| `DeleteOrderResult.cs` | sealed record | Empty marker. |
| `GetOrderResult.cs` | sealed record | `(Order Order)`. |
| `GetListOrderResult.cs` | sealed record | `(ImmutableList<Order> Orders)`. |
| `CreateOrderItemResult.cs` | sealed record | `(OrderItem OrderItem)` - the created item. |
| `UpdateOrderItemResult.cs` | sealed record | `(OrderItem OrderItem)` - the item after the update. |
| `DeleteOrderItemResult.cs` | sealed record | Empty marker. |
| `GetOrderItemResult.cs` | sealed record | `(OrderItem OrderItem)`. |
| `GetListOrderItemResult.cs` | sealed record | `(ImmutableList<OrderItem> OrderItems)`. |

## How it works

- Results contain only Contracts DTOs (`../Dto`), never domain types.
- Errors are not modelled here; they travel in the `Fin` failure channel as BuildingBlocks error types.

## Dependencies

- References: `../Dto`, `System.Collections.Immutable`.
- Used by: `../Commands`, `../Queries` (as `TResult`), Application handlers, `Template.Core.Api` gRPC response mappers, tests.

## Extending

Add `public sealed record <Request>Result(...)` alongside each new command/query.
