# CreateOrderItem

Command slice that creates a new order item (application layer).

## Contents

| Item | Kind | Description |
|---|---|---|
| `CreateOrderItemHandler.cs` | internal sealed class | `ICommandHandler<CreateOrderItemCommand, CreateOrderItemResult>`. |
| `CreateOrderItemValidator.cs` | internal sealed class | FluentValidation rules for `CreateOrderItemCommand`. |

## How it works

- Request: `CreateOrderItemCommand(Guid? OrderId, decimal? Quantity, decimal? Price)`. Response: `CreateOrderItemResult(OrderItem)` (DTO).
- Validation (pipeline): `OrderId` not null; `Quantity` not null and > 0; `Price` not null and >= 0.
- Handler steps:
  1. Map to `OrderId.From`, `Quantity.From`, `Price.From`; any null -> `ValidationError("Order id, quantity and price are required.")`.
  2. `OrderItem.Create(orderId, quantity, price)` (re-checks quantity/price invariants).
  3. `IOrderItemRepository.AddAsync` -> `IUnitOfWork.SaveChangesAsync`.
  4. Map with `DtoMappers.OrderItem`.
- The existence of the referenced order is not checked.
- Errors: `ValidationError`, `ConflictError` (id already exists), save errors from the unit of work (`OperationCancelled`, `ConcurrencyConflict`, `PersistenceFailed`).
- Events: `OrderItemCreated`, dispatched after a successful save.

## Dependencies

- References: Domain `OrderItems`, `Orders`, `ValueObjects`; Contracts; `BuildingBlocks`.
- Used by: mediator; `OrderItem.GrpcService.CreateOrderItem`; slice tests.
