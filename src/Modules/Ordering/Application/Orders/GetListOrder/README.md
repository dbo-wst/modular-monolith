# GetListOrder

Query slice that returns a page of orders (application layer, read side).

## Contents

| Item | Kind | Description |
|---|---|---|
| `GetListOrderHandler.cs` | internal sealed class | `IQueryHandler<GetListOrderQuery, GetListOrderResult>`. |

## How it works

- Request: `GetListOrderQuery(int PageNumber, int PageSize)`. Response: `GetListOrderResult(ImmutableList<Order>)`.
- Validation: none in the slice; the Persistence read store normalizes paging (page clamped to 1..1000, size <= 0 -> 10, max 100).
- Handler steps: `IOrderingReadStore.GetOrdersAsync(pageNumber, pageSize)` -> `ToImmutableList()` -> `Fin.Succ`.
- Errors: none returned by the handler itself (always succeeds; an empty page is a valid result).
- Events: none.

## Dependencies

- References: `IOrderingReadStore`; Contracts.
- Used by: mediator; `Order.GrpcService.GetOrders`; slice tests.
