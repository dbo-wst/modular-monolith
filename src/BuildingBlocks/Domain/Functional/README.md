# Functional

Functional-programming helpers (namespace `Acme.Core.BuildingBlocks.Domain.Functional`, project `Acme.Core.BuildingBlocks.Domain`) on top of LanguageExt `Fin<T>` / `Option<T>`, used to compose railway-style handler and value-object code.

## Contents

| Item | Kind | Description |
|---|---|---|
| `FinExtensions.cs` | static class (C# 14 `extension` block) | `Fin<A>` helpers: `ToUnit`, `MapError`, `Try` (func/action), `Bind` overloads |
| `FinAsyncExtensions.cs` | static class (extension block) | Same operations on `Task<Fin<A>>`: `ToUnitAsync`, `TryAsync`, `MapErrorAsync`, `MapAsync`, `BindAsync`, `BindForceAsync`, `MatchAsync` |
| `FuncFinAsyncExtensions.cs` | static class (extension block) | Same operations on a deferred `Func<Task<Fin<A>>>` (invokes it first) |
| `General.cs` | static class | `Exception.ToError()`, `Then`, `Unit.Success/Failure(Async)`, `ToFin`/`ToFinAsync` for values, `Option<A>` and `Task<Option<A>>` |
| `Advanced.cs` | static class | Partial application (`Applying`, up to 5 args) and `Currying` (2 and 3 args) for `Func<>` |
| `ListExtensions.cs` | static class | Empty placeholder (its only member is commented out) |

## How it works

- Uses C# 14 extension members (`extension<A>(Fin<A> that) { ... }`), including static extensions on `Unit` (`Unit.Success()`).
- `Bind`/`BindAsync` short-circuit on failure; the success function is called only for `Succ`.
- `BindForceAsync` always runs the continuation, but on failure returns the original error (useful for cleanup).
- `MapError(Func<Error>)` replaces the message with the new error's message and keeps the original error as inner.
- `Try` catches exceptions thrown by the function/action and returns them as a failed `Fin`.
- `ToFin()` on a value: `null` becomes `ValidationError` ("A value of type X is required."); `ToFin(error)` lets the caller choose the error. On `Option<A>`, `None` becomes a generic `Error` unless an explicit error is passed.

```csharp
// From Ordering DeleteOrderHandler
return orders.GetByIdAsync(orderId, ct)            // Task<Fin<Order>>
    .BindAsync(order => { order.Delete(); return orders.DeleteAsync(order, ct); })
    .BindAsync(_ => unitOfWork.SaveChangesAsync(ct))
    .MapAsync(_ => new DeleteOrderResult());
```

## Dependencies
- References: LanguageExt.Core; `Domain/Errors` (`ValidationError` in `General.ToFin`).
- Used by: Ordering Application handlers (`CreateOrderHandler`, `UpdateOrderHandler`, `GetOrderHandler`, `DeleteOrderHandler`, and the `OrderItems` equivalents), Ordering Domain value objects (`Quantity`, `OrderNumber`, `EffectiveDate`), `test/BuildingBlocks/Domain`.

## Extending
- Add helpers to the class matching the receiver type (`Fin`, `Task<Fin>`, `Func<Task<Fin>>`, general); keep the sync/async/deferred variants consistent.
- Keep helpers generic and side-effect free; module-specific logic belongs in the module.
