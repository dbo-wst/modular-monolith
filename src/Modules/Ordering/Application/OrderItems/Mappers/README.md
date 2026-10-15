# Order Item Mappers (Application)

Mapper from the `OrderItem` domain model to the Contracts DTO (application layer). Accessed as `DtoMappers.OrderItem`.

## Contents

| Item | Kind | Description |
|---|---|---|
| `OrderItemMapper.cs` | readonly struct, `IOnewayMap<Domain.OrderItems.OrderItem, Contracts.Dto.OrderItem>` | Domain aggregate -> `OrderItem` DTO (id, order id, quantity, price). |

## How it works

- Stateless struct; unwraps value objects (`Id.Value`, `OrderId.Value`, `Quantity.Value`, `Price.Value`).
- One-way only: commands never map a DTO back to the aggregate; queries get DTOs straight from the read store.

## Dependencies

- References: `BuildingBlocks.Domain` (`IOnewayMap`), Domain `OrderItems`, Contracts `Dto`.
- Used by: `CreateOrderItemHandler`, `UpdateOrderItemHandler` (via `DtoMappers.OrderItem`).

## Extending

Add new properties here when the DTO and aggregate grow; new mappers are exposed as `static readonly` fields in `DtoMappers`.
