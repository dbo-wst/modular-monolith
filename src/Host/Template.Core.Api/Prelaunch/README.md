# Prelaunch (gRPC services)

Namespace root `Template.Core.Api.Prelaunch`, mirroring the `prelaunch` proto package. Contains the shared error classification and one folder per module with its gRPC services.

## Contents

| Item | Kind | Description |
|---|---|---|
| `ErrorCodeClassifier.cs` | internal static class | `Classify(Error)` turns a domain error into the stable string code written to every `*Error.code` proto field. |
| [`Ordering/`](Ordering/README.md) | folder | Module wiring and gRPC services for `OrderService` and `OrderItemService`. |

## How it works

`ErrorCodeClassifier.Classify` looks for the first `AppError` (from `Template.Core.BuildingBlocks.Domain.Errors`) and returns its `ErrorCode`; otherwise `ErrorCodes.Internal`:

1. the error itself is an `AppError` -> its code;
2. a `ManyErrors` -> the first member (in order) that resolves to an `AppError`;
3. otherwise -> recurse into `error.Inner`, or `internal` if there is none.

Classification is by type only; messages are passed through unchanged. Codes are `not_found`, `validation`, `conflict`, `cancelled`, `internal` (`ErrorCodes`), which the gateway maps to HTTP 404/400/409/409/500.

## Dependencies
- References: `LanguageExt.Common`, `Template.Core.BuildingBlocks.Domain.Errors`.
- Used by: every `Mappers/<Op>/ErrorResponse.cs` in the module folders.

## Extending
Add `Prelaunch/<Module>/` following the `Ordering` layout and use `ErrorCodeClassifier.Classify` in its error-response mappers. New error categories belong in `BuildingBlocks.Domain` (`AppError` subtype + `ErrorCodes` constant), not here.
