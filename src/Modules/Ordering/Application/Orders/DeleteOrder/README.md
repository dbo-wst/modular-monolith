# DeleteOrder

Command slice that deletes an order (application layer).

## Contents

| Item | Kind | Description |
|---|---|---|
| `DeleteOrderHandler.cs` | internal sealed class | `ICommandHandler<DeleteOrderCommand, DeleteOrderResult>`. |

## How it works

- Request: `DeleteOrderCommand(Guid OrderId)`. Response: empty `DeleteOrderResult`.
- Validation: none.
- Handler steps:
  1. `IOrderRepository.GetByIdAsync`.
  2. `order.Delete()` (raises the event).
  3. `IOrderRepository.DeleteAsync` (removes the row and tracks the aggregate for event dispatch).
  4. `IUnitOfWork.SaveChangesAsync`.
- Errors: `NotFoundError` (unknown id), save errors from the unit of work.
- Events: `OrderDeleted`, dispatched after a successful save.
- Order items referencing the order are not touched (no relationship is configured).

## Dependencies

- References: Domain `Orders`; Contracts; `BuildingBlocks` (`IUnitOfWork`, `Fin` helpers).
- Used by: mediator; `Order.GrpcService.DeleteOrder`; slice tests.
