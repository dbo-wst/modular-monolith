# CLAUDE.md — BuildingBlocks

Shared kernel used by every module. It contains **mechanisms only, never business concepts** (no orders, customers,
module value objects). Changes here affect all modules: keep them backward compatible and covered by tests in
`test/BuildingBlocks`.

## Projects

| Project | May depend on | Contains |
|---|---|---|
| `Acme.Core.BuildingBlocks.Domain` | LanguageExt only | `AggregateRoot<TId>`, `DomainEvent`, `IHasDomainEvents`, `IUnitOfWork`, `IDomainEventDispatcher`, `IMap`/`IOnewayMap`, `Errors/`, `Functional/` |
| `Acme.Core.BuildingBlocks.Application` | Domain, FluentValidation, DI/Options abstractions | `IMediator` + pipeline, request/handler interfaces, `ValidationBehavior`, `DomainEventDispatcher`, `IDomainEventHandler<T>`, DI extensions, options validation |
| `Acme.Core.BuildingBlocks.Persistence` | Domain, EF Core | `UnitOfWork<TContext>`, `DomainEventCollector` |

Application must not reference Persistence or EF Core; Domain must not reference either of the others.

## Rules

- **Errors**: every expected error derives from `AppError` (an `Expected`) and carries one `ErrorCodes` constant.
  New error kinds: add a sealed record with a private constructor and static factories (`New`, domain-specific
  ones like `ForEntity`). Add a new `ErrorCodes` value only together with the host mappings (gRPC classifier,
  HTTP status) and docs. Never rename existing codes.
- **Mediator contract** (do not break):
  - `IRequest<out TResponse>`, `ICommand<T>`, `ICommand` (= `ICommand<Unit>`), `IQuery<T>`.
  - `IRequestHandler`/`ICommandHandler`/`IQueryHandler` return `Task<Fin<T>>`.
  - Exactly one handler per request type across all modules; `AddRequestHandlersFromAssembly` throws on duplicates.
  - Dispatch goes through the cached `RequestHandlerWrapper`. No per-call reflection; handler exceptions are not wrapped.
- **Pipeline order**: all `IPipelineBehavior` (registration order, outermost) -> all `ICommandPipelineBehavior`
  (commands only) -> handler. Behaviours return `Fin`, call `next()` at most once and must not swallow failures.
- **Registration**: `AddMediator()` is called once by the host (idempotent). Modules call only
  `AddRequestHandlersFromAssembly`. Command behaviours register via `AddCommandPipelineBehavior<T>()`.
- **Domain events**:
  - `DomainEvent` is an abstract record with `EventId`/`OccurredOn`. Aggregates raise events via the protected `Raise`.
  - Repositories `Track` aggregates in the scoped `DomainEventCollector`.
  - `UnitOfWork<TContext>` dispatches only after a successful `SaveChanges`, then clears the events.
  - Inside `ExecuteInTransactionAsync`, dispatch happens before commit, so a failing handler rolls the transaction back.
  - Handlers are matched by exact runtime type.
- **Unit of work** maps database exceptions to `ConcurrencyConflict`, `OperationCancelled`, `PersistenceFailed`,
  `TransactionFailed`, and uses the EF execution strategy for transactions.
- Everything public here needs XML doc comments describing the contract.
- An outbox, a message bus or new cross-cutting behaviours require an ADR first.
