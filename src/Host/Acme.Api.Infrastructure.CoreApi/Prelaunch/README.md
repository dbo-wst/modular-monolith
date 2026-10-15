# Prelaunch (gateway gRPC clients)

Namespace root `Acme.Api.Infrastructure.CoreApi.Prelaunch`, mirroring the `prelaunch` proto package and the `Acme.Core.Api.Prelaunch` server side. Holds one folder per module.

## Contents

| Item | Kind | Description |
|---|---|---|
| [`Ordering/`](Ordering/README.md) | folder | gRPC clients, DTOs and mappers for `OrderService` and `OrderItemService`. |

## How it works
Each module folder exposes an `Add<Module>GrpcClients` extension that `AddCoreApiInfrastructure` calls. Resource subfolders follow the same layout: `IGrpcApiClient`/`GrpcApiClient`, `<Resource>Dto`, `Requests/`, `Responses/`, `Mappers/`, `ProtoMappers`.

## Dependencies
- References: `Acme.Core.Protocol` generated clients (`Acme.Core.Protocol.Prelaunch.*`).
- Used by: `ServiceCollectionExtensions.AddCoreApiInfrastructure`, `Acme.Api` endpoints.

## Extending
Create `Prelaunch/<Module>/` by copying the `Ordering` layout and register it in `AddCoreApiInfrastructure`.
