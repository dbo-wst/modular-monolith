# CLAUDE.md

Acme is a .NET 10 **modular monolith** solution template: Clean Architecture inside each module, vertical slices in
the Application layer, CQRS through a custom `IMediator`, railway-oriented results with LanguageExt `Fin<T>`.
These rules are binding. If a request conflicts with them, say so and propose a compliant alternative instead of
silently diverging. Area-specific rules live in nested `CLAUDE.md` files (`src/BuildingBlocks`, `src/Modules`,
`src/Host`, `src/Protocol`, `test`, `test_integration`); every folder also has a `README.md` describing what is there.

## Commands

```bash
dotnet build Acme.Core.slnx                       # must end with 0 warnings, 0 errors
dotnet test Acme.Core.slnx --filter "FullyQualifiedName!~Ordering.Application.IntegrationTest&FullyQualifiedName!~Persistence.IntegrationTest"   # fast, no SQL
dotnet test Acme.Core.slnx                        # everything; SQL tests need Docker or Persistence:ConnectionString
SQL_CONNECTION_STRING="Server=x;Database=y;TrustServerCertificate=True" dotnet ef migrations has-pending-model-changes --project src/Modules/<Module>/Persistence
./add-migrations.sh                               # add module migrations (needs SQL_CONNECTION_STRING)
./clean-bin-obj.sh                                # remove build output
```

## Solution layout

```
src/BuildingBlocks/{Domain,Application,Persistence}   Acme.Core.BuildingBlocks.*  shared kernel (no business code)
src/BuildingBlocks/Observability           OTLP export of logs, traces and metrics (Acme.*, runtime) for the hosts (ADR 0002)
src/Modules/<Module>/{Domain,Application,Contracts,Persistence,<Adapter>}  Acme.Core.<Module>.*
src/Host/Acme.Core.Api                     gRPC host, composition root of all modules, Quartz background jobs
src/Host/Acme.Api                          HTTP gateway (REST -> gRPC), knows no module internals
src/Host/Acme.Api.Infrastructure.CoreApi   gateway's gRPC client adapter
src/Protocol                               .proto contracts between gateway and Core.Api
test/                                      fast unit + architecture tests
test_integration/                          host tests and SQL-backed tests
doc/                                       arc42 documentation + ADRs (docfx)
operations/                                CI (Dockerfiles) and CD (compose, scripts)
```

## Dependency rule (enforced by `test/Architecture`)

- Domain -> `BuildingBlocks.Domain` only. Never EF Core, `Microsoft.Extensions.DependencyInjection`, FluentValidation,
  Application, Contracts or adapters.
- Contracts -> `BuildingBlocks.Application` only (for `ICommand`/`IQuery`).
- Application -> Domain, Contracts, BuildingBlocks. Never adapters or EF Core.
- Adapters (`Persistence`, future `Messaging`, `ExternalServices`, ...) implement ports; one project per adapter;
  adapters never reference each other.
- Hosts reach a module only through its Contracts and its DI extension. The gateway never references
  module Domain/Application/adapters.
- A module never references another module's Domain/Application/adapters; integrate through Contracts.
- `BuildingBlocks.Domain` must not depend on the other building blocks.
- Never weaken or delete an architecture test to make a change pass. Fix the dependency instead.

## Global coding rules

- Expected failures are values: return `Fin<T>` with a typed `AppError` (`NotFoundError`, `ValidationError`,
  `ConflictError`, `CancelledError`, persistence errors). Do not throw for business or not-found cases.
- Classify errors by type / `ErrorCode` (`not_found`, `validation`, `conflict`, `cancelled`, `internal`), never by
  message text. Never change existing error code values: they are on the wire.
- Compose with `Bind`/`Map`/`BindAsync`/`MapAsync`/`Match`; do not unwrap `Fin` with exceptions or `.ThrowIfFail()` in
  production code.
- Commands, queries, results and DTOs are `sealed record`s. Handlers and validators are `internal sealed`.
- Always pass `CancellationToken` through.
- Nullable enabled; `ImplicitUsings`; file-scoped namespaces; namespace = assembly name + folder path.
- Package versions only in `Directory.Packages.props` (central package management); no `Version` in csproj.
  Pin vulnerable transitive packages there instead of suppressing NU19xx warnings.
- Keep the build at 0 warnings. Do not add `#pragma warning disable` / `NoWarn` without a stated reason.
- Match the style of neighbouring code; comments only where the intent is not obvious.

## Workflow for any change

1. Find the relevant `README.md` / nested `CLAUDE.md` and the closest existing example (Ordering is the reference
   module) and follow it.
2. Make the change in small steps; keep slices self-contained.
3. Add or update tests (unit first; integration where persistence or hosts are affected).
4. `dotnet build` (0 warnings) and the fast test run must be green. If the EF model changed, add a migration and
   check `has-pending-model-changes`.
5. Update the `README.md` of every touched folder and, for architectural changes, `doc/` (building block view,
   cross-cutting concepts) and an ADR in `doc/09-architecture-decisions`.
6. Do not commit unless asked.

## Naming

- Root prefix `Acme`; projects `Acme.Core.<Module>.<Layer>`; tests `<Project>.Test` / `<Project>.IntegrationTest`.
- Slice = `<Verb><Aggregate>` (`CreateOrder`, `GetListOrder`); types `<Slice>Command|Query`, `<Slice>Result`,
  `<Slice>Handler`, `<Slice>Validator`.
- Ops assets use `acme-*` (images, containers, volumes, `acme-network`).
