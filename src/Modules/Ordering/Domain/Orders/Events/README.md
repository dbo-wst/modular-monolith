# Order Events

Domain events raised by the `Order` aggregate (domain layer). They are collected on the aggregate and dispatched in-process by the unit of work after a successful save.

## Contents

| Item | Kind | Description |
|---|---|---|
| `OrderCreated.cs` | sealed record : `DomainEvent` | `OrderId`, `OrderNumber`, `EffectiveDate`; raised by `Order.Create`. |
| `OrderUpdated.cs` | sealed record : `DomainEvent` | `OrderId`, `Number`, `EffectiveDate`, `Status` (full state after the change); raised by `ChangeDetails` (only on real change) and every status transition. |
| `OrderDeleted.cs` | sealed record : `DomainEvent` | `OrderId`; raised by `Order.Delete()`. |

## How it works

- Events carry domain types (`OrderId`, value objects, `OrderStatus`), not primitives.
- `OrderRepository` tracks the aggregate in the scoped `DomainEventCollector`; `UnitOfWork.SaveChangesAsync` drains and dispatches the events via `IDomainEventDispatcher` only if the save succeeded.
- No production `IDomainEventHandler<>` for these events exists yet; they are consumed in tests (`test/Modules/Ordering/Application/DomainEvents`).

## Dependencies

- References: `BuildingBlocks.Domain` (`DomainEvent`), `../` (`OrderId`, `OrderStatus`), `../../ValueObjects`.
- Used by: `Order` aggregate; handlers registered via `AddRequestHandlersFromAssembly` (none in production code yet); tests.

## Extending

Add a `sealed record XxxHappened(...) : DomainEvent` here and raise it from a method of `Order`. To react to an event, implement `IDomainEventHandler<TEvent>` in an Application assembly; it is registered automatically by `AddRequestHandlersFromAssembly`.
