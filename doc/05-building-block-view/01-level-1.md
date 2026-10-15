# 5.1 Level 1

```
src/
  BuildingBlocks/Shared.Kernel      mediator, CQRS markers, pipeline behaviours, functional helpers
  Modules/Ordering/
    Domain/          aggregates, value objects, domain events, repository ports (IOrderRepository, IOrderItemRepository, IUnitOfWork)
    Application/     vertical slices (Orders/CreateOrder, ...), IOrderingReadStore port
    Persistence/     adapter: EF Core repositories, unit of work, read store, migrations
    <Adapter>/       further adapters as separate projects (Messaging, ExternalServices, ...)
    Contracts/       commands, queries, results, DTOs
  Protocol/          gRPC contracts
  Host/              Template.Api (HTTP gateway), Template.Core.Api (gRPC host)
```

Dependency rule: Domain <- Application <- adapters (Persistence, ...); adapters do not reference each other; hosts reference Contracts only (see [ADR 0001](../09-architecture-decisions/0001-modular-monolith-clean-architecture-cqrs.md)).
