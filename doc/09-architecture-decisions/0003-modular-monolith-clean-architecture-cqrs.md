# 3. Modular Monolith, Clean Architecture, Vertical Slices and CQRS

Date: 2026-10-15

## Status

Accepted

## Context

ADR 0002 chose a modular monolith with Clean Architecture and vertical slices. The solution needs a concrete, enforceable structure.

## Decision

- Each business module lives in `src/Modules/<Module>` and consists of four projects:
  `Domain` (aggregates, events, ports), `Application` (use cases), `Infrastructure` (EF Core, read store, migrations) and `Contracts` (commands, queries, results, DTOs).
- Dependencies point inwards: Domain depends on nothing; Application depends on Domain and Contracts; Infrastructure implements the ports; hosts (`src/Host`) talk to a module only through Contracts and its DI registration. This is verified by `test/Architecture`.
- Application is organised as **vertical slices**: one folder per use case (e.g. `Orders/CreateOrder`) holding the handler and its validator.
- **CQRS**: commands (`ICommand<T>`, aggregate + repositories + `IUnitOfWork` write model) and queries (`IQuery<T>`, `IOrderingReadStore` read model projected to DTOs) are separate types dispatched by `IMediator`. Pipeline behaviours (`IPipelineBehavior`, currently validation) wrap every request.
- Shared building blocks live in `src/BuildingBlocks`.

## Consequences

Module boundaries and the dependency rule are compile- and test-enforced; adding a feature means adding one slice folder; reads can evolve independently of the write model.
