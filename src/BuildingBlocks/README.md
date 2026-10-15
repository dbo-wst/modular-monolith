# BuildingBlocks

Shared, module-agnostic infrastructure that every module and host builds on. It is split by Clean Architecture layer into three projects (Domain, Application, Persistence), so each module layer references only the building block of its own layer.

## Contents

| Item | Kind | Description |
|---|---|---|
| [`Domain/`](Domain/README.md) | project folder (`Acme.Core.BuildingBlocks.Domain`) | Aggregate root, domain events, unit-of-work and event-dispatcher ports, mapping interfaces, typed errors, `Fin<T>` helpers |
| [`Application/`](Application/README.md) | project folder (`Acme.Core.BuildingBlocks.Application`) | Custom CQRS mediator, pipeline behaviours, domain event dispatcher, DI registration, FluentValidation-based options validation |
| [`Persistence/`](Persistence/README.md) | project folder (`Acme.Core.BuildingBlocks.Persistence`) | EF Core `UnitOfWork<TContext>` and the scoped `DomainEventCollector` |

## How it works

- Project graph: `Application -> Domain`, `Persistence -> Domain`. Application and Persistence do not reference each other.
- Results are expressed as LanguageExt `Fin<T>`; failures are typed `AppError` records carrying a stable error code (`not_found`, `validation`, `conflict`, `cancelled`, `internal`).
- Ports live in Domain (`IUnitOfWork`, `IDomainEventDispatcher`); implementations live in Persistence (`UnitOfWork<TContext>`) and Application (`DomainEventDispatcher`), wired together by DI.
- End-to-end request flow:
  1. The host calls `IMediator.SendAsync(request)`.
  2. Generic `IPipelineBehavior`s run (e.g. `ValidationBehavior`), then `ICommandPipelineBehavior`s (commands only), then the single handler.
  3. The handler changes aggregates; repositories `Track` them in the `DomainEventCollector`.
  4. `IUnitOfWork.SaveChangesAsync` saves and, only on success, dispatches the collected domain events to `IDomainEventHandler<T>`s in the same scope.

## Dependencies
- References: LanguageExt.Core, LanguageExt.Transformers (Domain); FluentValidation, Microsoft.Extensions.DependencyInjection.Abstractions, Microsoft.Extensions.Options (Application); Microsoft.EntityFrameworkCore (Persistence).
- Used by: `src/Modules/Ordering/*` (Domain, Application, Contracts, Persistence), `src/Host/Acme.Core.Api`, `src/Host/Acme.Api.Infrastructure.CoreApi`, tests in `test/BuildingBlocks`.

## Extending
- Put code here only if it is module-agnostic and needed by more than one module or host; module-specific code stays in its module.
- Place new code in the project matching its layer, and keep Domain free of infrastructure packages.
