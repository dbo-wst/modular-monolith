# Ordering Module

The Ordering bounded context: manages orders (number, effective date, status) and order items (order reference, quantity, price). It is the reference module of the template and consists of four projects, one per layer/adapter.

## Contents

| Item | Kind | Description |
|---|---|---|
| [`Domain/`](Domain/README.md) | project `Template.Core.Ordering.Domain` | `Order` and `OrderItem` aggregates, value objects, domain events, repository ports. |
| [`Contracts/`](Contracts/README.md) | project `Template.Core.Ordering.Contracts` | Public commands, queries, results and DTOs of the module. |
| [`Application/`](Application/README.md) | project `Template.Core.Ordering.Application` | CQRS vertical slices (handlers, validators), DTO mappers, read-store port. |
| [`Persistence/`](Persistence/readme.md) | project `Template.Core.Ordering.Persistence` | EF Core / SQL Server adapter: DbContext, repositories, read store, unit of work, migrations. |

## How it works

- Write side (commands): handler -> domain factory/behaviour method -> `IOrderRepository`/`IOrderItemRepository` -> `IUnitOfWork.SaveChangesAsync` -> domain events (`OrderCreated`, `OrderItemUpdated`, ...) dispatched after a successful save.
- Read side (queries): handler -> `IOrderingReadStore`, which projects database rows straight to Contracts DTOs without rehydrating aggregates.
- `Order` and `OrderItem` are two independent aggregates; an item references its order only by `OrderId` (no navigation, no foreign key configured).
- Order status lifecycle: `New -> InProgress -> Completed`; `New`/`InProgress -> Cancelled` or `Rejected`. Other transitions fail with `ValidationError`.
- Data lives in SQL Server schema `ordering` (tables `Orders`, `OrderItems`).

## Dependencies

- References: `src/BuildingBlocks/Domain`, `Application`, `Persistence` (per project, see the project READMEs).
- Used by: `src/Host/Template.Core.Api` (`Prelaunch/Ordering`: `AddOrderingModule` registers `AddOrderingApplication()` + `AddOrderingPersistence(configuration)`; gRPC services `Order.GrpcService`, `OrderItem.GrpcService`), `test/Architecture`, `test/Modules/Ordering/*`, `test_integration/Modules/Ordering/*`, `test_integration/Host/*`.

## Extending

- New use case: add a command/query + result in [`Contracts/`](Contracts/README.md) and a slice folder in [`Application/`](Application/README.md).
- New aggregate: add a folder under `Domain/` (aggregate, id, repository interface, `Events/`), a matching folder under `Persistence/` (Dbo, `Configuration`, mapper, repository) plus a `DbSet` and a migration, and slices under `Application/`.
- New adapter (e.g. messaging): new project `Template.Core.Ordering.<Adapter>` referencing Domain/Application only; register it in the host and add it to the `Adapters` array in `test/Architecture/ArchitectureTests.cs`.
