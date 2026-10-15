# Queries

Read-side requests of the Ordering module (Contracts layer). Each is a `sealed record` implementing `IQuery<TResult>`.

## Contents

| Item | Kind | Description |
|---|---|---|
| `GetOrderQuery.cs` | sealed record | `(Guid OrderId)` -> `GetOrderResult`. |
| `GetListOrderQuery.cs` | sealed record | `(int PageNumber, int PageSize)` -> `GetListOrderResult`. |
| `GetOrderItemQuery.cs` | sealed record | `(Guid OrderItemId)` -> `GetOrderItemResult`. |
| `GetListOrderItemQuery.cs` | sealed record | `(int PageNumber, int PageSize)` -> `GetListOrderItemResult`. |

## How it works

- Handled by the `Get*` / `GetList*` slices, which read through `IOrderingReadStore` (no aggregates).
- Paging values are passed through unchanged; the Persistence read store normalizes them (page 1..1000, size default 10, max 100).

## Dependencies

- References: `BuildingBlocks.Application.Communication` (`IQuery<T>`), `../Results`.
- Used by: Application query handlers; `Acme.Core.Api` gRPC request mappers; tests.

## Extending

Add `public sealed record Get<Something>Query(...) : IQuery<Get<Something>Result>;` (must be sealed) and the result in `../Results`.
