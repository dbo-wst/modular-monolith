# src

Root of all production code of the modular monolith template. It contains the shared kernel (`BuildingBlocks`), the business modules (`Modules`), the gRPC contract (`Protocol`) and the two runnable hosts (`Host`).

## Contents

| Item | Kind | Description |
|---|---|---|
| [`BuildingBlocks/`](BuildingBlocks/README.md) | folder | Shared kernel split by layer: `Acme.Core.BuildingBlocks.Domain` (aggregates, domain events, typed errors and `ErrorCodes`, `IMap`/`IOnewayMap`), `.Application` (in-process `IMediator`, pipeline behaviours, validation) and `.Persistence` (unit of work, domain event collection). |
| [`Modules/`](Modules/README.md) | folder | Business modules. Currently `Ordering` with `Contracts`, `Domain`, `Application` and `Persistence` projects (`Acme.Core.Ordering.*`). |
| [`Protocol/`](Protocol/README.md) | folder | `Acme.Core.Protocol`: `.proto` files and generated gRPC server/client stubs shared by both hosts. |
| [`Host/`](Host/README.md) | folder | Runnable processes: `Acme.Api` (HTTP gateway), `Acme.Api.Infrastructure.CoreApi` (gateway's gRPC client layer) and `Acme.Core.Api` (gRPC host that composes the modules). |

## How it works

A request travels through two processes:

1. **HTTP gateway** (`Host/Acme.Api`, port 6002 locally) receives a REST call, e.g. `POST /ordering/orders`, bound to a client request record from `Acme.Api.Infrastructure.CoreApi`.
2. **gRPC client** (`Host/Acme.Api.Infrastructure.CoreApi`) maps the record to a Protocol message and calls `OrderService` / `OrderItemService` at `CoreApi:BaseUrl`.
3. **Core.Api** (`Host/Acme.Core.Api`, port 6001, HTTP/2) receives the call in a `GrpcService`, maps the message to a module contract (`Acme.Core.Ordering.Contracts` command or query) and sends it through `IMediator`.
4. **Module** (`Modules/Ordering`): the mediator runs the pipeline behaviours (validation, command behaviours) and the handler, which uses the domain model and persistence (SQL Server, `Persistence:ConnectionString`).
5. The handler returns `Fin<T>`. Core.Api maps success to the `success`/`deleted` branch of the response `oneof`, or a domain error to the `error` branch with a stable code (`ErrorCodeClassifier` -> `ErrorCodes`).
6. The gateway maps the Protocol result back to `Fin<TResponse>` (errors become `ApiError`) and the endpoint returns 200/201/204 or a ProblemDetails with 400/404/409/500.

```
HTTP client -> Acme.Api (Minimal API) -> IGrpcApiClient -> gRPC -> Acme.Core.Api GrpcService -> IMediator -> Ordering handler -> DB
```

Dependency direction: hosts reference `Protocol`; `Acme.Core.Api` additionally references module `Application` and `Persistence` projects; modules reference only `BuildingBlocks`. The gateway never references modules; it only shares `Protocol` and `BuildingBlocks.Domain` (for `ErrorCodes`).

## Dependencies
- References: central package versions come from `Directory.Packages.props` at repository root.
- Used by: tests in `test/` (unit, architecture) and `test_integration/` (host integration tests).

## Extending
- New module: add `Modules/<Module>/{Contracts,Domain,Application,Persistence}`, protos under `Protocol/Protos/prelaunch/<module>/`, gRPC services in `Acme.Core.Api/Prelaunch/<Module>/`, clients in `Acme.Api.Infrastructure.CoreApi/Prelaunch/<Module>/` and endpoints in `Acme.Api/<Module>/`. See the host READMEs for the exact wiring.
