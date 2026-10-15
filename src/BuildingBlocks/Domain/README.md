# BuildingBlocks.Domain

Project `Acme.Core.BuildingBlocks.Domain` (namespace `Acme.Core.BuildingBlocks.Domain`). The domain-layer building block: base types and ports that module Domain projects depend on, with no infrastructure dependencies beyond LanguageExt.

## Contents

| Item | Kind | Description |
|---|---|---|
| `AggregateRoot.cs` | abstract class `AggregateRoot<TId>` | Base for aggregates: `Id` plus an internal list of raised domain events (`Raise`, `DomainEvents`, `ClearDomainEvents`) |
| `DomainEvent.cs` | abstract record | Base for domain events; each instance gets its own `EventId` (Guid) and `OccurredOn` (UTC) |
| `IHasDomainEvents.cs` | interface | Exposes `DomainEvents` and `ClearDomainEvents()`; implemented by `AggregateRoot<TId>` |
| `IDomainEventDispatcher.cs` | interface | Port for in-process event dispatch; implemented in BuildingBlocks.Application |
| `IUnitOfWork.cs` | interface | Port for committing one business operation: `SaveChangesAsync`, `ExecuteInTransactionAsync<T>` |
| `IMap.cs` | interface `IMap<TLeft, TRight>` | Pure two-way mapping (`Map` in both directions) |
| `IOnewayMap.cs` | interface `IOnewayMap<TLeft, TRight>` | Pure one-way mapping `TLeft -> TRight` |
| [`Errors/`](Errors/README.md) | folder | Typed `AppError` hierarchy and stable `ErrorCodes` |
| [`Functional/`](Functional/README.md) | folder | Extension methods for `Fin<T>`, `Option<T>` and functions |

## How it works

- Aggregates call the protected `Raise(event)`; events accumulate on the instance until something drains them (`DomainEventCollector` in BuildingBlocks.Persistence).
- `DomainEvent` is a record, but equality includes `EventId`, so two events with the same payload are distinct.
- `IUnitOfWork` and `IDomainEventDispatcher` are declared here so persistence adapters depend only on this project; contract: a successful `SaveChangesAsync` also dispatches the events of every aggregate touched in the scope; dispatch stops at the first failure.
- All results are `Fin<T>` / `Fin<Unit>` from LanguageExt.

```csharp
public sealed record SomethingHappened(OrderId OrderId) : DomainEvent;

public sealed class Order : AggregateRoot<OrderId>
{
    public void DoSomething() { /* change state */ Raise(new SomethingHappened(Id)); }
}
```

## Dependencies
- References: LanguageExt.Core, LanguageExt.Transformers (no project references).
- Used by: BuildingBlocks.Application, BuildingBlocks.Persistence, `Acme.Core.Ordering.Domain` (`Order`, `OrderItem` aggregates), Ordering Application/Persistence (`IMap`/`IOnewayMap` mappers, `IUnitOfWork`), `Acme.Core.Api` and `Acme.Api.Infrastructure.CoreApi` (gRPC/HTTP mappers implementing `IOnewayMap`/`IMap`), `test/BuildingBlocks/Domain`.

## Extending
- New domain event: derive a `sealed record` from `DomainEvent` in the module's Domain project and `Raise` it from the aggregate.
- New aggregate: derive from `AggregateRoot<TId>` with a module-specific id type.
- New mapper: implement `IOnewayMap`/`IMap` as a (readonly) struct next to the code that needs it.
