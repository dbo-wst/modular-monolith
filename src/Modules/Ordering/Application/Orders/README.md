# Orders (Application)

Vertical slices for the `Order` aggregate (application layer): one folder per command/query plus the domain-to-DTO mappers.

## Contents

| Item | Kind | Description |
|---|---|---|
| [`CreateOrder/`](CreateOrder/README.md) | folder (slice) | `CreateOrderCommand` -> new order in status `New`. |
| [`UpdateOrder/`](UpdateOrder/README.md) | folder (slice) | `UpdateOrderCommand` -> change number/date and/or status. |
| [`DeleteOrder/`](DeleteOrder/README.md) | folder (slice) | `DeleteOrderCommand` -> remove an order. |
| [`GetOrder/`](GetOrder/README.md) | folder (slice) | `GetOrderQuery` -> one order DTO. |
| [`GetListOrder/`](GetListOrder/README.md) | folder (slice) | `GetListOrderQuery` -> page of order DTOs. |
| [`Mappers/`](Mappers/README.md) | folder | `OrderMapper`, `StatusMapper` (domain <-> Contracts DTO). |

## How it works

- Commands use `IOrderRepository` + `IUnitOfWork`; queries use `IOrderingReadStore`.
- Command results contain the order mapped via `DtoMappers.Order` after a successful save (delete returns an empty result).
- Only `CreateOrder` has a FluentValidation validator; the other slices rely on domain rules.

## Dependencies

- References: Domain `Orders`, `ValueObjects`; Contracts `Commands`, `Queries`, `Results`, `Dto`; `BuildingBlocks.Application.Communication`.
- Used by: `IMediator` (via `AddOrderingApplication`), called from `src/Host/Acme.Core.Api/Prelaunch/Ordering/Order/GrpcService.cs`; tests in `test/Modules/Ordering/Application/Orders` and `test_integration/Modules/Ordering/Application/Orders`.

## Extending

Add a folder `<Verb>Order/` with `<Verb>OrderHandler` (and optional `<Verb>OrderValidator`) for a new Contracts command/query.
