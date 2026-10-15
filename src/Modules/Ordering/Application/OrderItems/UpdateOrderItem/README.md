# UpdateOrderItem

Command slice that changes an order item (application layer).

## Contents

| Item | Kind | Description |
|---|---|---|
| `UpdateOrderItemHandler.cs` | internal sealed class | `ICommandHandler<UpdateOrderItemCommand, UpdateOrderItemResult>`. |

## How it works

- Request: `UpdateOrderItemCommand(Guid OrderItemId, Guid? OrderId, decimal? Quantity, decimal? Price)`; null fields mean "keep". Response: `UpdateOrderItemResult(OrderItem)` (DTO).
- Validation: no FluentValidation validator; `OrderItem.ChangeDetails` enforces quantity > 0 and price >= 0 on the resulting values.
- Handler steps:
  1. `IOrderItemRepository.GetByIdAsync`.
  2. `item.ChangeDetails(OrderId.From(...), Quantity.From(...), Price.From(...))`.
  3. `IOrderItemRepository.UpdateAsync` -> `IUnitOfWork.SaveChangesAsync`.
  4. Map with `DtoMappers.OrderItem`.
- Moving an item to another `OrderId` is allowed; the target order is not checked.
- Errors: `NotFoundError` (unknown id), `ValidationError` (invariant violation), save errors from the unit of work.
- Events: `OrderItemUpdated` if at least one value changed, dispatched after save.

## Dependencies

- References: Domain `OrderItems`, `Orders`, `ValueObjects`; Contracts; `BuildingBlocks`.
- Used by: mediator; `OrderItem.GrpcService.UpdateOrderItem`; slice tests.
