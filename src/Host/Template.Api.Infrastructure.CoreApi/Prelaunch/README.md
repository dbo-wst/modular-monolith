# Prelaunch (gateway gRPC clients)

Namespace root `Template.Api.Infrastructure.CoreApi.Prelaunch`, mirroring the `prelaunch` proto package and the `Template.Core.Api.Prelaunch` server side. Holds one folder per module.

## Contents

| Item | Kind | Description |
|---|---|---|
| [`Ordering/`](Ordering/README.md) | folder | gRPC clients, DTOs and mappers for `OrderService` and `OrderItemService`. |

## How it works
Each module folder exposes an `Add<Module>GrpcClients` extension that `AddCoreApiInfrastructure` calls. Resource subfolders follow the same layout: `IGrpcApiClient`/`GrpcApiClient`, `<Resource>Dto`, `Requests/`, `Responses/`, `Mappers/`, `ProtoMappers`.

## Dependencies
- References: `Template.Core.Protocol` generated clients (`Template.Core.Protocol.Prelaunch.*`).
- Used by: `ServiceCollectionExtensions.AddCoreApiInfrastructure`, `Template.Api` endpoints.

## Extending
Create `Prelaunch/<Module>/` by copying the `Ordering` layout and register it in `AddCoreApiInfrastructure`.
