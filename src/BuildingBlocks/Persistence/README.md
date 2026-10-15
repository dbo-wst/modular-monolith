# BuildingBlocks.Persistence

Project `Acme.Core.BuildingBlocks.Persistence` (namespace `Acme.Core.BuildingBlocks.Persistence`). The infrastructure-layer building block for EF Core: a reusable unit of work that maps database exceptions to typed errors and dispatches domain events after a successful save.

## Contents

| Item | Kind | Description |
|---|---|---|
| `Acme.Core.BuildingBlocks.Persistence.csproj` | project | References Microsoft.EntityFrameworkCore and BuildingBlocks.Domain |
| `UnitOfWork.cs` | abstract class `UnitOfWork<TContext>` | EF Core implementation of `IUnitOfWork`; modules derive from it to bind their `DbContext` |
| `DomainEventCollector.cs` | sealed class | Scoped registry of aggregates touched in the current unit of work |

## How it works

Why a collector: repositories map aggregates to persistence models, so EF Core's change tracker never sees aggregates or their events. Repositories therefore call `collector.Track(aggregate)` for every aggregate they add, update or delete (tracked by reference, duplicates ignored; events raised after `Track` are still picked up).

`SaveChangesAsync`:
1. Calls `Context.SaveChangesAsync`.
2. Maps exceptions: `OperationCanceledException` -> `OperationCancelled` (`cancelled`), `DbUpdateConcurrencyException` -> `ConcurrencyConflict` (`conflict`), `DbUpdateException` -> `PersistenceFailed` (`internal`). On failure nothing is dispatched.
3. On success, `collector.Drain()` returns pending events of all tracked aggregates (tracking order), clears them on the aggregates and forgets the aggregates.
4. Returns `IDomainEventDispatcher.DispatchAsync(events)`: handlers run in-process in the same scope; the first handler failure becomes the result.

`ExecuteInTransactionAsync(work)`:
1. Runs inside `Context.Database.CreateExecutionStrategy()` (retry-on-failure compatible).
2. Begins a transaction, runs `work`; `Succ` -> commit, `Fail` -> rollback; result is returned.
3. Any exception -> rollback and `TransactionFailed` (`internal`).
4. If `work` calls `SaveChangesAsync`, domain events are dispatched inside the transaction before commit, so a failing handler makes `work` fail and rolls the transaction back.

```csharp
// Module binding (Ordering)
public sealed class UnitOfWork(OrderingDbContext context, DomainEventCollector collector, IDomainEventDispatcher dispatcher)
    : UnitOfWork<OrderingDbContext>(context, collector, dispatcher);

// Registration
services.TryAddScoped<DomainEventCollector>();
services.AddScoped<IUnitOfWork, UnitOfWork>();
```

## Dependencies
- References: Microsoft.EntityFrameworkCore; project `Acme.Core.BuildingBlocks.Domain` (`IUnitOfWork`, `IDomainEventDispatcher`, `IHasDomainEvents`, `Errors`). The `IDomainEventDispatcher` implementation comes from BuildingBlocks.Application, registered by the host via `AddMediator()`.
- Used by: `Acme.Core.Ordering.Persistence` (`UnitOfWork`, `OrderRepository`, `OrderItemRepository`, `ServiceCollectionExtensions`).

## Extending
- New module: derive `UnitOfWork<YourDbContext>`, register it as scoped `IUnitOfWork`, register `DomainEventCollector` with `TryAddScoped`, and share one `DbContext` per scope between the unit of work and repositories.
- Every repository write method must call `DomainEventCollector.Track(aggregate)`, otherwise that aggregate's events are never dispatched.
