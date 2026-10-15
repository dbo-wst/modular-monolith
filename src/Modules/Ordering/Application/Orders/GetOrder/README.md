# GetOrder

Query slice that returns a single order (application layer, read side).

## Contents

| Item | Kind | Description |
|---|---|---|
| `GetOrderHandler.cs` | internal sealed class | `IQueryHandler<GetOrderQuery, GetOrderResult>`. |

## How it works

- Request: `GetOrderQuery(Guid OrderId)`. Response: `GetOrderResult(Order)` (DTO).
- Validation: none.
- Handler steps: `IOrderingReadStore.GetOrderAsync(orderId)` -> wrap the DTO in `GetOrderResult`. No aggregate is loaded.
- Errors: `NotFoundError` from the read store when the order does not exist.
- Events: none.

## Dependencies

- References: `IOrderingReadStore`; Contracts.
- Used by: mediator; `Order.GrpcService.GetOrder`; slice tests.
