# Order Items (Application)

Vertical slices for the `OrderItem` aggregate (application layer): one folder per command/query plus the domain-to-DTO mapper.

## Contents

| Item | Kind | Description |
|---|---|---|
| [`CreateOrderItem/`](CreateOrderItem/README.md) | folder (slice) | `CreateOrderItemCommand` -> new item. |
| [`UpdateOrderItem/`](UpdateOrderItem/README.md) | folder (slice) | `UpdateOrderItemCommand` -> change order id, quantity and/or price. |
| [`DeleteOrderItem/`](DeleteOrderItem/README.md) | folder (slice) | `DeleteOrderItemCommand` -> remove an item. |
| [`GetOrderItem/`](GetOrderItem/README.md) | folder (slice) | `GetOrderItemQuery` -> one item DTO. |
| [`GetListOrderItem/`](GetListOrderItem/README.md) | folder (slice) | `GetListOrderItemQuery` -> page of item DTOs. |
| [`Mappers/`](Mappers/README.md) | folder | `OrderItemMapper` (domain -> Contracts DTO). |

## How it works

- Commands use `IOrderItemRepository` + `IUnitOfWork`; queries use `IOrderingReadStore`.
- Items reference an order by id only; no slice checks that the referenced order exists.
- Only `CreateOrderItem` has a FluentValidation validator; updates rely on the domain invariants (quantity > 0, price >= 0).

## Dependencies

- References: Domain `OrderItems`, `Orders` (`OrderId`), `ValueObjects`; Contracts; `BuildingBlocks.Application.Communication`.
- Used by: `IMediator` (via `AddOrderingApplication`), called from `src/Host/Template.Core.Api/Prelaunch/Ordering/OrderItem/GrpcService.cs`; tests in `test/Modules/Ordering/Application/OrderItems` and `test_integration/Modules/Ordering/Application/OrderItems`.

## Extending

Add a folder `<Verb>OrderItem/` with `<Verb>OrderItemHandler` (and optional validator) for a new Contracts command/query.
