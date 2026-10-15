# Modules

Business modules of the modular monolith. Each module is a bounded context split into one project per Clean Architecture layer (Domain, Application, Contracts) plus one project per adapter (currently only Persistence). Modules are hosted in-process by `Acme.Core.Api` and share a single mediator from `src/BuildingBlocks`.

## Contents

| Item | Kind | Description |
|---|---|---|
| [`Ordering/`](Ordering/README.md) | folder (module) | Orders and order items: aggregates, CQRS slices, public contracts and EF Core persistence. |

## How it works

Layout of a module (`<Module>` = e.g. `Ordering`):

| Project | Assembly | Role | References |
|---|---|---|---|
| `Domain/` | `Acme.Core.<Module>.Domain` | Aggregates, value objects, domain events, repository ports | `BuildingBlocks.Domain` |
| `Contracts/` | `Acme.Core.<Module>.Contracts` | Public API of the module: commands, queries, results, DTOs | `BuildingBlocks.Application` |
| `Application/` | `Acme.Core.<Module>.Application` | One vertical slice (handler + optional validator) per command/query, read-store port | Domain, Contracts, `BuildingBlocks.Application` |
| `Persistence/` | `Acme.Core.<Module>.Persistence` | EF Core adapter: DbContext, repositories, read store, unit of work | Domain, Application, `BuildingBlocks.Persistence` |

Rules (enforced by `test/Architecture/ArchitectureTests.cs`):

- Domain depends on neither Application, Contracts nor adapters, and not on EF Core, `Microsoft.Extensions.DependencyInjection` or FluentValidation.
- Contracts depend on no other layer of the module.
- Application depends neither on adapters nor on EF Core.
- Adapters do not depend on each other; adapter assemblies live in the `Acme.Core.<Module>.` namespace.
- The HTTP gateway (`Acme.Api`) knows a module only through Contracts or `src/Protocol`, never through Domain/Application/adapters.
- Commands and queries in Contracts are `sealed`.

Request flow: gRPC service in `Acme.Core.Api` -> `IMediator.SendAsync(command/query)` -> validation behaviour (FluentValidation) -> handler -> repository/read store -> `IUnitOfWork.SaveChangesAsync` -> domain events dispatched in-process. Every step returns LanguageExt `Fin<T>`; expected failures are typed errors (`ValidationError`, `NotFoundError`, `ConflictError`, ...) from `BuildingBlocks.Domain.Errors`, not exceptions.

## Dependencies

- References: each module project references only its own layers and `src/BuildingBlocks/*` (see table above).
- Used by: `src/Host/Acme.Core.Api` (references Application + Persistence of each module and registers them), `test/Architecture`, `test/Modules/<Module>`, `test_integration/Modules/<Module>`, `test_integration/Host/*`.

## Extending

Adding a new module `Billing` (follow `Ordering/` as the reference):

1. Create `src/Modules/Billing/Domain`, `Contracts`, `Application` and one project per adapter (`Persistence`, later e.g. `Messaging`), named `Acme.Core.Billing.<Layer>.csproj`. Copy the project references from the Ordering `.csproj` files; target framework and nullable settings come from `Directory.Build.props`, package versions from `Directory.Packages.props`.
2. Domain: aggregates deriving from `AggregateRoot<TId>`, value objects, events deriving from `DomainEvent`, repository interfaces returning `Fin<T>`.
3. Contracts: `sealed record` commands (`ICommand<TResult>`) and queries (`IQuery<TResult>`), results and DTOs.
4. Application: one folder per slice with an `internal sealed` handler (`ICommandHandler`/`IQueryHandler`) and optional `AbstractValidator<T>`; expose `AddBillingApplication()` calling `services.AddRequestHandlersFromAssembly(...)`. Do not call `AddMediator()` in the module.
5. Persistence: own DbContext with its own schema and migrations history table, repositories that `Track` aggregates in `DomainEventCollector`, a `UnitOfWork : UnitOfWork<BillingDbContext>`; expose `AddBillingPersistence(IServiceCollection, IConfiguration)`.
6. Host: add project references to `src/Host/Acme.Core.Api/Acme.Core.Api.csproj`, create `Prelaunch/Billing/ServiceCollectionExtensions.cs` with `AddBillingModule` (calls both `Add...` methods) and the gRPC mapping, and call it from `Program.cs` after `AddMediator()`. Add `.proto` files under `src/Protocol/Protos` if the module is exposed over gRPC.
7. Architecture tests: reference the new projects from `test/Architecture/Acme.Core.Architecture.Test.csproj` and add equivalent rules for the new module (the existing rules are written against the Ordering assemblies; adapter assemblies go into the `Adapters` array).
8. Add all new projects (including test projects) to `Acme.Core.slnx` under `/src/Modules/Billing/` and `/test/Modules/Billing/` / `/test_integration/Modules/Billing/`.
