# 1. Modular Monolith, Clean Architecture, Vertical Slices and CQRS

## Status

Accepted

## Context

The template needs a concrete, enforceable structure that stays easy to change and extend as a solution grows.

## Decision

- Each business module lives in `src/Modules/<Module>` and consists of four projects:
  `Domain` (aggregates, value objects, events, ports), `Application` (use cases), `Infrastructure` (EF Core, read store, migrations) and `Contracts` (commands, queries, results, DTOs).
- Dependencies point inwards: Domain depends on nothing; Application depends on Domain and Contracts; Infrastructure implements the ports; hosts (`src/Host`) talk to a module only through Contracts and its DI registration. This is verified by `test/Architecture`.
- Application is organised as **vertical slices**: one folder per use case (e.g. `Orders/CreateOrder`) holding the handler and its validator.
- **CQRS**: commands (`ICommand<T>`, aggregate + repositories + `IUnitOfWork` write model) and queries (`IQuery<T>`, `IOrderingReadStore` read model projected to DTOs) are separate types dispatched by `IMediator`. Pipeline behaviours (`IPipelineBehavior`, currently validation) wrap every request.
- Shared building blocks live in `src/BuildingBlocks`.

## Consequences

Module boundaries and the dependency rule are compile- and test-enforced; adding a feature means adding one slice folder; reads can evolve independently of the write model.
