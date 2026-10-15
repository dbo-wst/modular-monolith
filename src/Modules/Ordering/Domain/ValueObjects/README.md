# Value Objects

Immutable value objects shared by the Ordering aggregates (domain layer). Each is a C# `record` wrapping a single primitive, giving equality by value and type safety.

## Contents

| Item | Kind | Description |
|---|---|---|
| `OrderNumber.cs` | record (`string Value`) | `MaybeCreate(string)` -> `Fin`; `From(string?)` returns null for null/blank input. |
| `EffectiveDate.cs` | record (`DateOnly Value`) | `Create` -> `Fin`; `From(DateOnly)`, `From(DateOnly?)`; `Today()` (UTC date). |
| `Quantity.cs` | record (`decimal Value`) | `Create` -> `Fin`; `From(decimal?)`. |
| `Price.cs` | record (`decimal Value`) | `Create` fails with `ValidationError` for negative values; `From(decimal)`, `From(decimal?)`. |

## How it works

- Nullable `From(...)` overloads map optional request fields: a missing value becomes `null`, which aggregates interpret as "required" (create) or "keep" (update).
- Only `Price.Create` validates on its own. The other rules live in the aggregates: order number not blank (`Order`), quantity > 0 and price >= 0 (`OrderItem`).
- `Create`/`MaybeCreate` use `ToFin()` from `BuildingBlocks.Domain.Functional`.

## Dependencies

- References: `BuildingBlocks.Domain` (`Functional`, `Errors`), LanguageExt.
- Used by: `Orders/Order`, `OrderItems/OrderItem`, domain events, Application handlers, Persistence mappers.

## Extending

Add a `record Xxx(T Value)` with a `Create`/`MaybeCreate` returning `Fin<Xxx>` (put self-contained validation there, as in `Price`) and nullable `From` helpers for mapping optional inputs.
