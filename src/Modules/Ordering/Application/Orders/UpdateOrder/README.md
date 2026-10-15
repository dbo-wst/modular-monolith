# UpdateOrder

Command slice that changes an order's details and/or status (application layer).

## Contents

| Item | Kind | Description |
|---|---|---|
| `UpdateOrderHandler.cs` | internal sealed class | `ICommandHandler<UpdateOrderCommand, UpdateOrderResult>`. |

## How it works

- Request: `UpdateOrderCommand(Guid OrderId, string? Number, DateOnly? EffectiveDate, EStatus? Status)`; null fields mean "keep". Response: `UpdateOrderResult(Order)` (DTO).
- Validation: no FluentValidation validator; rules come from the domain.
- Handler steps:
  1. `IOrderRepository.GetByIdAsync`.
  2. `order.ChangeDetails(number, date)` (blank number ignored).
  3. If `Status` is set and differs from the current one, map it with `DtoMappers.Status` and call `Start()` (`InProgress`), `Complete()`, `Cancel()` or `Reject()`; target `New` is never allowed.
  4. `IOrderRepository.UpdateAsync` -> `IUnitOfWork.SaveChangesAsync`.
  5. Map with `DtoMappers.Order`.
- Errors: `NotFoundError` (unknown id), `ValidationError` (illegal status transition, "Cannot change order status from X to Y."), save errors from the unit of work.
- Events: `OrderUpdated` per actual change (details change and status transition each raise one), dispatched after save.

## Dependencies

- References: Domain `Orders`, `ValueObjects`; Contracts; `Mappers/StatusMapper` via `DtoMappers`.
- Used by: mediator; `Order.GrpcService.UpdateOrder`; slice tests.
