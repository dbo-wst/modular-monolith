# 8.8 Error Handling

Expected failures are returned as `Fin<T>` (LanguageExt) with errors from `BuildingBlocks/Domain/Errors` (namespace `Acme.Core.BuildingBlocks.Domain.Errors`); hosts map them to HTTP / gRPC error responses.

Every expected error derives from `AppError` (a LanguageExt `Expected`) and carries a stable `ErrorCode` from `ErrorCodes` (`not_found`, `validation`, `conflict`, `cancelled`, `internal`): `NotFoundError`, `ValidationError`, `ConflictError` (incl. `ConcurrencyConflict`), `CancelledError` (incl. `OperationCancelled`), `PersistenceFailed` / `TransactionFailed` (internal). Messages are free human-readable text; consumers classify by type only. The Core API's `ErrorCodeClassifier` takes the first `AppError` found (the error, the members of a `ManyErrors`, or an inner error) and falls back to `internal`; the gateway maps the code to an HTTP status.
