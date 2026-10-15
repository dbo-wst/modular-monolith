# GetOrderItem

Query slice that returns a single order item (application layer, read side).

## Contents

| Item | Kind | Description |
|---|---|---|
| `GetOrderItemHandler.cs` | internal sealed class | `IQueryHandler<GetOrderItemQuery, GetOrderItemResult>`. |

## How it works

- Request: `GetOrderItemQuery(Guid OrderItemId)`. Response: `GetOrderItemResult(OrderItem)` (DTO).
- Validation: none.
- Handler steps: `IOrderingReadStore.GetOrderItemAsync(orderItemId)` -> wrap the DTO in `GetOrderItemResult`. No aggregate is loaded.
- Errors: `NotFoundError` from the read store when the item does not exist.
- Events: none.

## Dependencies

- References: `IOrderingReadStore`; Contracts.
- Used by: mediator; `OrderItem.GrpcService.GetOrderItem`; slice tests.
