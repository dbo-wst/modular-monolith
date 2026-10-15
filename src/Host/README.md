# Host

Runnable processes and the gateway-side integration layer. The HTTP gateway (`Template.Api`) talks to the module host (`Template.Core.Api`) exclusively over gRPC using the contract in `src/Protocol`.

## Contents

| Item | Kind | Description |
|---|---|---|
| [`Template.Api/`](Template.Api/README.md) | project (web) | HTTP/REST gateway with OpenAPI + Scalar; maps `/ordering/...` routes to gRPC clients. Assembly `Template.Api`. |
| [`Template.Api.Infrastructure.CoreApi/`](Template.Api.Infrastructure.CoreApi/README.md) | project (library) | Typed gRPC clients (`IGrpcApiClient`), client request/response records, Protocol mappers and `ApiError`. Assembly `Template.Api.Infrastructure.CoreApi`. |
| [`Template.Core.Api/`](Template.Core.Api/README.md) | project (web, gRPC) | gRPC host; registers the shared mediator and the Ordering module and exposes `OrderService` / `OrderItemService`. Assembly `Template.Core.Api`. |

## How it works

| Process | Local URL (`launchSettings.json`) | Protocol | Key config |
|---|---|---|---|
| `Template.Api` | `http://localhost:6002` | HTTP/1.1 REST | `CoreApi:BaseUrl`, optional `PathBase`, `Serilog` |
| `Template.Core.Api` | `http://localhost:6001` | HTTP/2 only (`Kestrel:EndpointDefaults:Protocols`) | `Persistence:ConnectionString`, `Serilog` |

- Both hosts use the `Local` environment in launch profiles, so `appsettings.Local.json` supplies `CoreApi:BaseUrl` (gateway) and `Persistence:ConnectionString` (core).
- Both use a Serilog bootstrap logger, then `UseSerilog` reading the `Serilog` section (console + rolling files under `logs/`).
- Error flow: module `Fin` failure -> `ErrorCodeClassifier` code in the proto `*Error` message -> `ApiError` in the gateway -> HTTP status via `Endpoints.StatusFor`.

## Dependencies
- References: `src/Protocol` (both sides), `src/BuildingBlocks/Domain` (gateway, for `ErrorCodes`), `src/Modules/Ordering/{Application,Persistence}` (core host).
- Used by: `test_integration/Host/Template.Api.IntegrationTest`, `test_integration/Host/Template.Core.Api.IntegrationTest`.

## Extending
For a new resource or module, add in this order: protos (`src/Protocol`), gRPC service + mappers (`Template.Core.Api/Prelaunch/<Module>`), client + mappers (`Template.Api.Infrastructure.CoreApi/Prelaunch/<Module>`), endpoints (`Template.Api/<Module>`), then wire the `Add...`/`Map...` extension methods in both `Program.cs` files.
