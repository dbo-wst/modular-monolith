# Orders (Domain)

The `Order` aggregate of the Ordering domain layer: identity, status lifecycle, invariants and the repository port for orders.

## Contents

| Item | Kind | Description |
|---|---|---|
| `Order.cs` | class (aggregate root) | Order with `OrderNumber`, `EffectiveDate`, `Status`; factories and behaviour methods. |
| `OrderId.cs` | record | Strongly typed `Guid` id with `From`, nullable `From`, `MaybeCreate`. |
| `OrderStatus.cs` | enum | `New`, `InProgress`, `Completed`, `Cancelled`, `Rejected`. |
| `IOrderRepository.cs` | interface | `AddAsync`, `UpdateAsync`, `DeleteAsync`, `GetByIdAsync`, all returning `Task<Fin<...>>`. |
| [`Events/`](Events/README.md) | folder | `OrderCreated`, `OrderUpdated`, `OrderDeleted`. |

## How it works

- `Order.Create(number, effectiveDate)`: fails with `ValidationError` if the number is null/blank ("Order number is required.") or the date is null; otherwise assigns a new `OrderId`, sets status `New`, raises `OrderCreated`.
- `Order.Rehydrate(id, ...)`: loads persisted state without validation; missing values default to empty number, `default` date and `New`.
- `ChangeDetails(number?, effectiveDate?)`: null means keep; a blank number is ignored. Raises `OrderUpdated` only if a value actually changed. Always succeeds (returns `Order`, not `Fin`).
- Status transitions (each raises `OrderUpdated` with the full current state):

  | Method | Target | Allowed from |
  |---|---|---|
  | `Start()` | `InProgress` | `New` |
  | `Complete()` | `Completed` | `InProgress` |
  | `Cancel()` | `Cancelled` | `New`, `InProgress` |
  | `Reject()` | `Rejected` | `New`, `InProgress` |

  Any other transition returns `ValidationError("Cannot change order status from X to Y.")`. `Completed`, `Cancelled` and `Rejected` are terminal.
- `Delete()` only raises `OrderDeleted`; the repository removes the row.

## Dependencies

- References: `BuildingBlocks.Domain` (`AggregateRoot<TId>`, `ValidationError`), `../ValueObjects`, LanguageExt.
- Used by: Application `Orders/*` slices and `Orders/Mappers`, Persistence `Orders/` (`OrderRepository`, `OrderMapper`, `EStatusDboMapper`), `OrderItems` (items reference `OrderId`), domain tests.

## Extending

Add a new status by extending `OrderStatus`, a transition method using `Transition(target, allowedFrom...)`, and the matching values in `Contracts/Dto/EStatus`, `Application/Orders/Mappers/StatusMapper`, `Persistence/Orders/EStatusDbo` and `EStatusDboMapper`.
