# Application Abstractions

Ports (interfaces) owned by the Ordering application layer and implemented by an adapter. Currently only the query-side read store.

## Contents

| Item | Kind | Description |
|---|---|---|
| `IOrderingReadStore.cs` | interface | Returns Contracts DTOs directly: `GetOrderAsync`, `GetOrdersAsync`, `GetOrderItemAsync`, `GetOrderItemsAsync`. |

## How it works

- Note: the interface lives in namespace `Template.Core.Ordering.Application` (not `.Abstractions`).
- Single-item reads return `Task<Fin<Dto>>` (a `NotFoundError` when missing); list reads return `Task<IReadOnlyList<Dto>>` for a `pageNumber`/`pageSize` page and never fail.
- It bypasses domain aggregates entirely (CQRS read side); write-side ports (`IOrderRepository`, `IOrderItemRepository`) live in Domain, `IUnitOfWork` in BuildingBlocks.Domain.

## Dependencies

- References: `Template.Core.Ordering.Contracts` (`Dto.Order`, `Dto.OrderItem`), LanguageExt.
- Used by: `GetOrderHandler`, `GetListOrderHandler`, `GetOrderItemHandler`, `GetListOrderItemHandler`; implemented by `Persistence/ReadStore.cs` (registered scoped in `AddOrderingPersistence`); test fakes in `test/Modules/Ordering`.

## Extending

Add a new port here when a handler needs infrastructure (e.g. an external service); implement it in an adapter project and register it in that adapter's `Add...` extension. Keep signatures in terms of Contracts DTOs or domain types and `Fin<T>`.
