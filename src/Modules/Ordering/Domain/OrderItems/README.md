# Order Items (Domain)

The `OrderItem` aggregate of the Ordering domain layer: a line with quantity and price that references an order by id, plus its repository port.

## Contents

| Item | Kind | Description |
|---|---|---|
| `OrderItem.cs` | class (aggregate root) | Item with `OrderId`, `Quantity`, `Price`; factories and behaviour methods. |
| `OrderItemId.cs` | record | Strongly typed `Guid` id with `From`, nullable `From`, `MaybeCreate`. |
| `IOrderItemRepository.cs` | interface | `AddAsync`, `UpdateAsync`, `DeleteAsync`, `GetByIdAsync`, all returning `Task<Fin<...>>`. |
| [`Events/`](Events/README.md) | folder | `OrderItemCreated`, `OrderItemUpdated`, `OrderItemDeleted`. |

## How it works

- `OrderItem` is its own aggregate root, not a child entity of `Order`; it holds only the `OrderId`. The domain does not check that the referenced order exists.
- Invariants (checked in `Create` and `ChangeDetails`): quantity must be greater than zero, price must not be negative. Violations return `ValidationError`.
- `OrderItem.Create(orderId, quantity, price)`: also fails if `orderId` is null; assigns a new `OrderItemId` and raises `OrderItemCreated`.
- `OrderItem.Rehydrate(id, ...)`: loads persisted state without validation; missing values default to `Guid.Empty` order id, quantity 0, price 0.
- `ChangeDetails(orderId?, quantity?, price?)`: null means keep; validates the resulting quantity/price, then raises `OrderItemUpdated` only if a value changed.
- `Delete()` only raises `OrderItemDeleted`; the repository removes the row.

## Dependencies

- References: `BuildingBlocks.Domain`, `../Orders` (`OrderId`), `../ValueObjects`, LanguageExt.
- Used by: Application `OrderItems/*` slices and `OrderItems/Mappers`, Persistence `OrderItems/` (`OrderItemRepository`, `OrderItemMapper`), domain tests.

## Extending

Add new item properties as value objects, extend `Create`/`Rehydrate`/`ChangeDetails` and the `OrderItemCreated`/`OrderItemUpdated` payloads, then update `OrderItemDbo`, `Configuration`, both mappers, the Contracts DTO and add a migration.
