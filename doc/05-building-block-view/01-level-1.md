# 5.1 Level 1

```
src/
  BuildingBlocks/Shared.Kernel      mediator, CQRS markers, pipeline behaviours, functional helpers
  Modules/Ordering/
    Domain/          aggregates, value objects, domain events, repository ports (IOrderRepository, IOrderItemRepository, IUnitOfWork)
    Application/     vertical slices (Orders/CreateOrder, ...), IOrderingReadStore port
    Infrastructure/  EF Core write store, read store, migrations
    Contracts/       commands, queries, results, DTOs
  Protocol/          gRPC contracts
  Host/              Template.Api (HTTP gateway), Template.Core.Api (gRPC host)
```

Dependency rule: Domain <- Application <- Infrastructure; hosts reference Contracts only (see ADR 0003).
