# BuildingBlocks.Application

Project `Acme.Core.BuildingBlocks.Application` (namespace `Acme.Core.BuildingBlocks.Application.*`). The application-layer building block: the in-process CQRS mediator with its pipeline, the domain event dispatcher, DI registration helpers, and FluentValidation-based options validation.

## Contents

| Item | Kind | Description |
|---|---|---|
| `Acme.Core.BuildingBlocks.Application.csproj` | project | References FluentValidation, DI abstractions, Options and BuildingBlocks.Domain |
| [`Communication/`](Communication/README.md) | folder | `IMediator`/`Mediator`, request/command/query and handler interfaces, pipeline behaviours, `ValidationBehavior`, `DomainEventDispatcher`, `AddMediator`/`AddRequestHandlersFromAssembly` |
| [`Validation/`](Validation/README.md) | folder | `FluentValidateOptions<TOptions>` and `ValidateFluentValidation()` for `IOptions` validation |

## How it works

- The host calls `AddMediator()` once; it registers the single shared `IMediator`, the `IDomainEventDispatcher` implementation and the default `ValidationBehavior`.
- Each module's Application project calls `AddRequestHandlersFromAssembly(assembly)` to register its handlers, domain event handlers and validators.
- Every request returns `Task<Fin<TResponse>>`; failures are typed errors from `BuildingBlocks.Domain.Errors`.
- This project implements the `IDomainEventDispatcher` port declared in BuildingBlocks.Domain, which `UnitOfWork<TContext>` (BuildingBlocks.Persistence) calls after a successful save.

## Dependencies
- References: FluentValidation, Microsoft.Extensions.DependencyInjection.Abstractions, Microsoft.Extensions.Options; project `Acme.Core.BuildingBlocks.Domain`.
- Used by: `Acme.Core.Ordering.Application`, `Acme.Core.Ordering.Contracts` (commands/queries implement `ICommand`/`IQuery`), `Acme.Core.Ordering.Persistence` (via `ValidateFluentValidation`), `Acme.Core.Api` (`AddMediator`, `IMediator`), `test/BuildingBlocks/Application`.
