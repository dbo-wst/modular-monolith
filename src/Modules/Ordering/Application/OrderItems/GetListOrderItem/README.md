# GetListOrderItem

Query slice that returns a page of order items (application layer, read side).

## Contents

| Item | Kind | Description |
|---|---|---|
| `GetListOrderItemHandler.cs` | internal sealed class | `IQueryHandler<GetListOrderItemQuery, GetListOrderItemResult>`. |

## How it works

- Request: `GetListOrderItemQuery(int PageNumber, int PageSize)`. Response: `GetListOrderItemResult(ImmutableList<OrderItem>)`.
- Validation: none in the slice; the Persistence read store normalizes paging (page clamped to 1..1000, size <= 0 -> 10, max 100).
- Handler steps: `IOrderingReadStore.GetOrderItemsAsync(pageNumber, pageSize)` -> `ToImmutableList()` -> `Fin.Succ`.
- The list is not filtered by order; it pages over all items.
- Errors: none returned by the handler itself (always succeeds).
- Events: none.

## Dependencies

- References: `IOrderingReadStore`; Contracts.
- Used by: mediator; `OrderItem.GrpcService.GetOrderItems`; slice tests.
