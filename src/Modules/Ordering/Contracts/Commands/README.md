# Commands

Write-side requests of the Ordering module (Contracts layer). Each is a `sealed record` implementing `ICommand<TResult>`.

## Contents

| Item | Kind | Description |
|---|---|---|
| `CreateOrderCommand.cs` | sealed record | `(string? Number, DateOnly? EffectiveDate)` -> `CreateOrderResult`. |
| `UpdateOrderCommand.cs` | sealed record | `(Guid OrderId, string? Number, DateOnly? EffectiveDate, EStatus? Status)` -> `UpdateOrderResult`. |
| `DeleteOrderCommand.cs` | sealed record | `(Guid OrderId)` -> `DeleteOrderResult`. |
| `CreateOrderItemCommand.cs` | sealed record | `(Guid? OrderId, decimal? Quantity, decimal? Price)` (all default null) -> `CreateOrderItemResult`. |
| `UpdateOrderItemCommand.cs` | sealed record | `(Guid OrderItemId, Guid? OrderId, decimal? Quantity, decimal? Price)` -> `UpdateOrderItemResult`. |
| `DeleteOrderItemCommand.cs` | sealed record | `(Guid OrderItemId)` -> `DeleteOrderItemResult`. |

## How it works

- Create commands: nullable fields are required in practice (validators and handlers reject nulls).
- Update commands: the id is mandatory, other fields are optional; null means "keep the current value". `UpdateOrderCommand.Status` triggers a domain status transition.
- Handled by the slices in `Application/Orders/*` and `Application/OrderItems/*`.

## Dependencies

- References: `BuildingBlocks.Application.Communication` (`ICommand<T>`), `../Results`, `../Dto` (`EStatus`).
- Used by: Application command handlers and validators; `Acme.Core.Api` gRPC request mappers; tests.

## Extending

Add `public sealed record <Verb><Entity>Command(...) : ICommand<<Verb><Entity>Result>;` (must be sealed, enforced by architecture tests) plus the result in `../Results`.
