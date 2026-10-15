# Ordering Application

Application layer of the Ordering module (assembly `Acme.Core.Ordering.Application`). It implements every command and query from Contracts as a vertical slice (handler + optional FluentValidation validator) and defines the read-store port used by queries.

## Contents

| Item | Kind | Description |
|---|---|---|
| [`Abstractions/`](Abstractions/README.md) | folder | `IOrderingReadStore` query-side port. |
| [`Orders/`](Orders/README.md) | folder | Order slices (Create/Update/Delete/Get/GetList) and domain-to-DTO mappers. |
| [`OrderItems/`](OrderItems/README.md) | folder | Order item slices and mapper. |
| `DtoMappers.cs` | static class | Singleton mapper instances: `DtoMappers.Order`, `DtoMappers.Status`, `DtoMappers.OrderItem`. |
| `ServiceCollectionExtensions.cs` | static class | `AddOrderingApplication()` registers all handlers and validators of this assembly. |
| `Acme.Core.Ordering.Application.csproj` | project | References Domain, Contracts, `BuildingBlocks.Application`. |

## How it works

- Handlers implement `ICommandHandler<TCommand, TResult>` / `IQueryHandler<TQuery, TResult>` and return `Task<Fin<TResult>>`. They are `internal sealed`; tests get access via `InternalsVisibleTo` (`Acme.Core.Ordering.Application.Test`, `...Application.IntegrationTest`).
- Pipeline (from BuildingBlocks): `IMediator.SendAsync` -> `ValidationBehavior` runs all `IValidator<TRequest>`; any failure returns a single `ValidationError` (messages joined with `"; "`) and the handler is not called.
- Command handlers: build/load aggregate -> domain method -> repository -> `IUnitOfWork.SaveChangesAsync` -> map aggregate to DTO with `DtoMappers`. Domain events are dispatched by the unit of work after the save.
- Query handlers: call `IOrderingReadStore` and wrap the DTOs in a result; no aggregates involved.
- Composition uses the `Fin` async helpers from `BuildingBlocks.Domain.Functional` (`AsTask`, `BindAsync`, `MapAsync`); the first failure short-circuits the chain.
- `AddOrderingApplication()` calls `AddRequestHandlersFromAssembly`; the shared `IMediator` and pipeline behaviours are registered once by the host via `AddMediator()`.

## Dependencies

- References: `Acme.Core.Ordering.Domain`, `Acme.Core.Ordering.Contracts`, `Acme.Core.BuildingBlocks.Application` (FluentValidation, LanguageExt via BuildingBlocks.Domain); packages `Microsoft.Extensions.DependencyInjection.Abstractions`, `Microsoft.Extensions.Logging.Abstractions`.
- Used by: `Acme.Core.Ordering.Persistence` (implements `IOrderingReadStore`), `src/Host/Acme.Core.Api` (`AddOrderingModule`), `test/Modules/Ordering/Application`, `test_integration/Modules/Ordering/Application`, `test/Architecture`.

## Extending

1. Add the command/query and result to `Contracts/Commands|Queries|Results`.
2. Create a slice folder (e.g. `Orders/ApproveOrder/`) with an `internal sealed` handler and, if the request has input rules, an `internal sealed` `AbstractValidator<TRequest>`.
3. Nothing to register manually: `AddRequestHandlersFromAssembly` picks up handlers, validators and domain event handlers. Only one handler per request type is allowed.
4. Never reference EF Core or the Persistence project (enforced by architecture tests); add a port interface instead.
