# DeleteOrderItem

Command slice that deletes an order item (application layer).

## Contents

| Item | Kind | Description |
|---|---|---|
| `DeleteOrderItemHandler.cs` | internal sealed class | `ICommandHandler<DeleteOrderItemCommand, DeleteOrderItemResult>`. |

## How it works

- Request: `DeleteOrderItemCommand(Guid OrderItemId)`. Response: empty `DeleteOrderItemResult`.
- Validation: none.
- Handler steps:
  1. `IOrderItemRepository.GetByIdAsync`.
  2. `item.Delete()` (raises the event).
  3. `IOrderItemRepository.DeleteAsync` (removes the row and tracks the aggregate for event dispatch).
  4. `IUnitOfWork.SaveChangesAsync`.
- Errors: `NotFoundError` (unknown id), save errors from the unit of work.
- Events: `OrderItemDeleted`, dispatched after a successful save.

## Dependencies

- References: Domain `OrderItems`; Contracts; `BuildingBlocks`.
- Used by: mediator; `OrderItem.GrpcService.DeleteOrderItem`; slice tests.
