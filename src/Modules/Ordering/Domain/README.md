# Ordering Domain

Domain layer of the Ordering module (assembly `Template.Core.Ordering.Domain`). Contains the `Order` and `OrderItem` aggregates, their strongly typed ids, value objects, domain events and repository ports. It is framework-free: no EF Core, DI or FluentValidation.

## Contents

| Item | Kind | Description |
|---|---|---|
| [`Orders/`](Orders/README.md) | folder | `Order` aggregate, `OrderId`, `OrderStatus`, `IOrderRepository`, order events. |
| [`OrderItems/`](OrderItems/README.md) | folder | `OrderItem` aggregate, `OrderItemId`, `IOrderItemRepository`, item events. |
| [`ValueObjects/`](ValueObjects/README.md) | folder | `OrderNumber`, `EffectiveDate`, `Quantity`, `Price`. |
| `Template.Core.Ordering.Domain.csproj` | project | References `BuildingBlocks.Domain` only. |

## How it works

- Aggregates derive from `AggregateRoot<TId>` (BuildingBlocks) and collect `DomainEvent`s via `Raise(...)`; persistence dispatches them after a successful save.
- New aggregates are created via `Create(...)` factories returning `Fin<T>` (invariant violations become `ValidationError`); persisted state is loaded via `Rehydrate(...)`, which never validates and falls back to neutral defaults for missing values.
- State changes go through behaviour methods (`ChangeDetails`, `Start`, `Complete`, `Cancel`, `Reject`, `Delete`) that raise the corresponding events.
- Repository interfaces return `Task<Fin<T>>`; their implementations live in the Persistence adapter.

## Dependencies

- References: `Template.Core.BuildingBlocks.Domain` (which brings `LanguageExt.Core`).
- Used by: `Template.Core.Ordering.Application`, `Template.Core.Ordering.Persistence`, `test/Modules/Ordering/Domain`, `test/Architecture`, test companions.

## Extending

- New value object: a `record` in `ValueObjects/` with `Create`/`MaybeCreate` returning `Fin<T>` and nullable `From(...)` helpers (see existing ones).
- New aggregate: a folder with the aggregate (`AggregateRoot<TId>`, private constructor, `Create` + `Rehydrate`), an id record, a repository interface and an `Events/` folder with `sealed record ... : DomainEvent`.
- Keep infrastructure out: the architecture tests fail if this assembly references EF Core, DI or FluentValidation.
