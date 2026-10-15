# Order Mappers (Application)

Mappers between the `Order` domain model and the Contracts DTOs (application layer). Accessed through the static `DtoMappers` class.

## Contents

| Item | Kind | Description |
|---|---|---|
| `OrderMapper.cs` | readonly struct, `IOnewayMap<Domain.Orders.Order, Contracts.Dto.Order>` | Domain aggregate -> `Order` DTO (id, number, effective date, status). |
| `StatusMapper.cs` | readonly struct, `IMap<OrderStatus, EStatus>` | Two-way mapping between domain `OrderStatus` and contract `EStatus`. |

## How it works

- Mappers are stateless structs; single instances are exposed as `DtoMappers.Order` and `DtoMappers.Status`.
- `OrderMapper` unwraps value objects (`Id.Value`, `OrderNumber.Value`, `EffectiveDate.Value`) and maps status with `DtoMappers.Status`.
- `StatusMapper` maps value by value (`New`, `InProgress`, `Completed`, `Cancelled`, `Rejected`) and throws `ArgumentOutOfRangeException` for unknown values.
- Used only on the write side (command results); queries get DTOs straight from the read store.

## Dependencies

- References: `BuildingBlocks.Domain` (`IMap`, `IOnewayMap`), Domain `Orders`, Contracts `Dto`.
- Used by: `CreateOrderHandler`, `UpdateOrderHandler` (via `DtoMappers`).

## Extending

Add a new mapper struct implementing `IOnewayMap<,>` or `IMap<,>` and expose it as a `static readonly` field in `DtoMappers`.
