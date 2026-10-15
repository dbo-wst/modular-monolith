# 5.1 Level 1

```
src/
  BuildingBlocks/
    Domain/          aggregate root, domain events, expected errors, functional (LanguageExt) helpers, IMap (LanguageExt only)
    Application/     mediator, CQRS markers, pipeline behaviours, FluentValidation options validation
    Persistence/     EF Core UnitOfWork<TContext> base (save/transaction with error mapping)
  Modules/Ordering/
    Domain/          aggregates, value objects, domain events, repository ports (IOrderRepository, IOrderItemRepository, IUnitOfWork)
    Application/     vertical slices (Orders/CreateOrder, ...), IOrderingReadStore port
    Persistence/     adapter: EF Core repositories, unit of work, read store, migrations
    <Adapter>/       further adapters as separate projects (Messaging, ExternalServices, ...)
    Contracts/       commands, queries, results, DTOs
  Protocol/          gRPC contracts
  Host/              Acme.Api (HTTP gateway), Acme.Core.Api (gRPC host)
```

Dependency rule: Domain <- Application <- adapters (Persistence, ...); adapters do not reference each other; hosts reference Contracts only (see [ADR 0001](../09-architecture-decisions/0001-modular-monolith-clean-architecture-cqrs.md)). BuildingBlocks follow the same layering: modules reference the building block of their own layer (Domain -> BuildingBlocks.Domain, Application/Contracts -> BuildingBlocks.Application, Persistence -> BuildingBlocks.Persistence); BuildingBlocks.Domain stays free of EF Core, DI and FluentValidation.
