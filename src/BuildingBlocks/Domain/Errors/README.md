# Errors

Typed application errors (namespace `Acme.Core.BuildingBlocks.Domain.Errors`, project `Acme.Core.BuildingBlocks.Domain`). Every expected failure returned in a `Fin<T>` should be one of these records, so callers can classify failures by type or error code instead of parsing messages.

## Contents

| Item | Kind | Description |
|---|---|---|
| `ErrorCodes.cs` | static class | Stable string codes: `not_found`, `validation`, `conflict`, `cancelled`, `internal`; `Of(Error)` classifies any error |
| `AppError.cs` | abstract record | Base of all typed errors; derives from LanguageExt `Expected` and adds `ErrorCode` |
| `NotFoundError.cs` | sealed record | Code `not_found`; `New(message)`, `ForEntity<T>(entity)` |
| `ValidationError.cs` | sealed record | Code `validation`; `New(message)` |
| `ConflictError.cs` | record | Code `conflict`; `New(message)`, `AlreadyExists<T>(entity)`; open for derivation |
| `ConcurrencyConflict.cs` | sealed record : `ConflictError` | Optimistic concurrency failure; wraps the exception as inner error |
| `CancelledError.cs` | record | Code `cancelled`; `New(message)`; open for derivation |
| `OperationCancelled.cs` | sealed record : `CancelledError` | Operation cancelled; wraps the exception as inner error |
| `PersistenceFailed.cs` | sealed record | Code `internal`; failed to persist changes (wraps exception) |
| `TransactionFailed.cs` | sealed record | Code `internal`; transaction failed (wraps exception) |

## How it works

- `ErrorCodes` values go on the wire (gRPC `*Error.Code`) and are mapped to HTTP status codes by the gateway, so they must never change.
- `AppError` passes `message` and optional `inner` error to `Expected` (numeric code is always `0`); `ErrorCode` is the classification key. `Message` is free human-readable text and must not be parsed.
- Constructors are private/protected; instances are created via static factories (`New`, `ForEntity`, `AlreadyExists`).
- Exception-wrapping errors (`ConcurrencyConflict`, `OperationCancelled`, `PersistenceFailed`, `TransactionFailed`) use a fixed `ErrorMessage` constant and keep the exception as inner `Error`.
- Where they are produced: `ValidationBehavior` returns `ValidationError`; `UnitOfWork<TContext>` maps `OperationCanceledException`, `DbUpdateConcurrencyException`, `DbUpdateException` and transaction exceptions to the four exception-wrapping errors.

```csharp
if (order is null)
    return Fin<Order>.Fail(NotFoundError.ForEntity(orderId));

// consumer side: classify by type/code, never by message
// first AppError in the error, a ManyErrors or the inner error; otherwise "internal"
var code = ErrorCodes.Of(error);
```

## Dependencies
- References: LanguageExt.Core (`Expected`, `Error`, `Option`).
- Used by: BuildingBlocks.Application (`ValidationBehavior`), BuildingBlocks.Persistence (`UnitOfWork<TContext>`), Ordering Domain/Application/Persistence, `Acme.Core.Api` (`Prelaunch/ErrorCodeClassifier.cs` calls `ErrorCodes.Of`), BuildingBlocks.Application (`FailureLoggingBehavior` uses `ErrorCodes.Of` to pick the log level).

## Extending
- A new specific error: derive a `sealed record` from the matching category (`ConflictError`, `CancelledError`) or from `AppError` with an existing code, with a private constructor and a static `New` factory.
- A new category requires a new `ErrorCodes` constant plus matching mapping in the hosts (gRPC classifier and HTTP gateway); never change existing values.
