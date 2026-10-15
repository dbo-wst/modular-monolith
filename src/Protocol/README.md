# Protocol

gRPC contract between the HTTP gateway and the module host (assembly `Acme.Core.Protocol`, class library). It compiles the `.proto` files into C# messages plus server and client stubs and provides small conversion helpers.

## Contents

| Item | Kind | Description |
|---|---|---|
| [`Protos/`](Protos/README.md) | folder | `.proto` sources (`ProtoRoot="Protos"`), currently `prelaunch/ordering/order` and `prelaunch/ordering/order_item`. |
| `BytesExtension.cs` | static class | `Guid.AsByteString()` and `ByteString.AsGuid()` (ids travel as `bytes`, 16-byte `Guid.ToByteArray()` layout). |
| `TimestampExtension.cs` | static class | `DateOnly.AsTimestamp()` (midnight UTC) and `Timestamp.AsDateOnly()`. |
| `Acme.Core.Protocol.csproj` | project | Lists every proto as a `<Protobuf>` item. |

## How it works
- `Grpc.Tools` generates code at build time; namespaces come from `option csharp_namespace` (`Acme.Core.Protocol.Prelaunch.Ordering.Order` / `.OrderItem`).
- Only `service.proto` files are compiled with `GrpcServices="Server,Client"`; other files produce messages only. The same assembly therefore serves both `Acme.Core.Api` (servers) and `Acme.Api.Infrastructure.CoreApi` (clients).
- Every response uses a `oneof result` with a success branch and an `*Error { code, message }` branch; `code` holds a value from `ErrorCodes` (see `Protos/README.md`).

## Dependencies
- References: packages `Google.Protobuf`, `Grpc.Net.Client`, `Grpc.Tools` (private assets).
- Used by: `Acme.Core.Api`, `Acme.Api.Infrastructure.CoreApi`.

## Extending
Add the new `.proto` files under `Protos/prelaunch/<module>/<resource>/` and register each in `Acme.Core.Protocol.csproj` with `ProtoRoot="Protos"`; add `GrpcServices="Server,Client"` only for the `service.proto`.
