# Order Item Events

Domain events raised by the `OrderItem` aggregate (domain layer). They are dispatched in-process by the unit of work after a successful save.

## Contents

| Item | Kind | Description |
|---|---|---|
| `OrderItemCreated.cs` | sealed record : `DomainEvent` | `OrderItemId`, `OrderId`, `Quantity`, `Price`; raised by `OrderItem.Create`. |
| `OrderItemUpdated.cs` | sealed record : `DomainEvent` | Same fields, full state after the change; raised by `ChangeDetails` only when a value changed. |
| `OrderItemDeleted.cs` | sealed record : `DomainEvent` | `OrderItemId`; raised by `OrderItem.Delete()`. |

## How it works

- Events carry domain types (`OrderItemId`, `OrderId`, `Quantity`, `Price`).
- `OrderItemRepository` tracks the aggregate in `DomainEventCollector`; `UnitOfWork.SaveChangesAsync` dispatches the collected events only after a successful save.
- No production `IDomainEventHandler<>` for these events exists yet; they are verified in tests.

## Dependencies

- References: `BuildingBlocks.Domain` (`DomainEvent`), `../` (`OrderItemId`), `../../Orders` (`OrderId`), `../../ValueObjects`.
- Used by: `OrderItem` aggregate; tests.

## Extending

Add a `sealed record ... : DomainEvent` here, raise it from an `OrderItem` method, and handle it with an `IDomainEventHandler<TEvent>` in an Application assembly.
