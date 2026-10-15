# CreateOrder

Command slice that creates a new order (application layer).

## Contents

| Item | Kind | Description |
|---|---|---|
| `CreateOrderHandler.cs` | internal sealed class | `ICommandHandler<CreateOrderCommand, CreateOrderResult>`. |
| `CreateOrderValidator.cs` | internal sealed class | FluentValidation rules for `CreateOrderCommand`. |

## How it works

- Request: `CreateOrderCommand(string? Number, DateOnly? EffectiveDate)`. Response: `CreateOrderResult(Order)` (DTO).
- Validation (pipeline): `Number` not empty, max 128 chars; `EffectiveDate` not null.
- Handler steps:
  1. Map to `OrderNumber.From` / `EffectiveDate.From`; if either is null -> `ValidationError("Order number and effective date are required.")`.
  2. `Order.Create(number, date)` (status `New`).
  3. `IOrderRepository.AddAsync` -> `IUnitOfWork.SaveChangesAsync`.
  4. Map with `DtoMappers.Order`.
- Errors: `ValidationError` (validator or domain), `ConflictError` (id already exists), save errors from the unit of work (`OperationCancelled`, `ConcurrencyConflict`, `PersistenceFailed`).
- Events: `OrderCreated`, dispatched after a successful save.

## Dependencies

- References: Domain `Orders`, `ValueObjects`; Contracts; `BuildingBlocks` (`IUnitOfWork`, `ValidationError`, `Fin` helpers).
- Used by: mediator; `Order.GrpcService.CreateOrder` in `Template.Core.Api`; slice tests.
