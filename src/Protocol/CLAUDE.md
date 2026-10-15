# CLAUDE.md — Protocol

`Acme.Core.Protocol` holds the gRPC contracts between the HTTP gateway (`Acme.Api`) and the core host (`Acme.Core.Api`).
These are wire contracts: treat every change as a potential breaking change.

## Rules

- Layout: `Protos/prelaunch/<module>/<aggregate>/*.proto`, package `acme.core.protocol.prelaunch.<module>.<aggregate>`,
  `csharp_namespace` `Acme.Core.Protocol.Prelaunch.<Module>.<Aggregate>`.
- Every response is a `oneof` holding a success branch (or `deleted`) and an `error` branch. The error branch carries
  `code`, one of the stable `ErrorCodes` values, and `message`.
- Type conventions:
  - Ids are `bytes` (Guid).
  - Dates are `google.protobuf.Timestamp`.
  - Decimals are `string` (invariant culture).
  - Enums have a `*_UNSPECIFIED = 0` first value.
- Never renumber or reuse field numbers. Mark removed fields as `reserved`. Add fields instead of changing their type.
- Proto types never leak into modules. Only the host mappers convert proto messages to Contracts and back.
- After changing a proto, rebuild and update both sides' mappers and the host integration tests in the same change.
