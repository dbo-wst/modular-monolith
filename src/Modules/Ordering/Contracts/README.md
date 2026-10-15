# Ordering Contracts

Public API of the Ordering module (assembly `Template.Core.Ordering.Contracts`): the commands, queries, results and DTOs that callers send through `IMediator`. It depends on no other layer of the module, so hosts and other modules can reference it without seeing the domain.

## Contents

| Item | Kind | Description |
|---|---|---|
| [`Commands/`](Commands/README.md) | folder | Create/Update/Delete commands for orders and order items. |
| [`Queries/`](Queries/README.md) | folder | Get-by-id and paged list queries. |
| [`Results/`](Results/README.md) | folder | One result record per command/query. |
| [`Dto/`](Dto/README.md) | folder | `Order`, `OrderItem` DTOs and the `EStatus` enum. |
| [`Events/`](Events/README.md) | folder (empty, `.gitkeep`) | Placeholder for integration events. |
| [`Responses/`](Responses/README.md) | folder (empty, `.gitkeep`) | Placeholder. |
| `Template.Core.Ordering.Contracts.csproj` | project | References `BuildingBlocks.Application` only (for `ICommand<T>` / `IQuery<T>`). |

## How it works

- Every request is a `sealed record` implementing `ICommand<TResult>` or `IQuery<TResult>`; the result type ties it to exactly one handler in Application.
- Fields use primitives (`Guid`, `string`, `decimal`, `DateOnly`) and are mostly nullable so that "not provided" can be expressed (required on create, "keep" on update).
- Handlers return `Fin<TResult>`; failures are BuildingBlocks error types, not part of this project.
- Architecture tests enforce: no dependency on Domain, Application or adapters; all types in `*.Commands` / `*.Queries` namespaces are sealed.

## Dependencies

- References: `Template.Core.BuildingBlocks.Application`.
- Used by: `Template.Core.Ordering.Application` (handlers, mappers, `IOrderingReadStore`), `Template.Core.Ordering.Persistence` (read store DTOs), `src/Host/Template.Core.Api` (gRPC `ProtoMappers` map protobuf <-> contracts, transitively via the Application reference), tests.

## Extending

Add a command/query in `Commands/` or `Queries/` and its result in `Results/`, using DTOs from `Dto/`; then implement the slice in `Application/`.
