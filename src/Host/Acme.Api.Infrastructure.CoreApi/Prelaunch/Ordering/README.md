# Ordering (gateway gRPC clients)

Gateway clients for the Ordering module, namespace `Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering`. Registers the generated `OrderServiceClient` / `OrderItemServiceClient` and the typed wrappers used by `Acme.Api` endpoints.

## Contents

| Item | Kind | Description |
|---|---|---|
| `ServiceCollectionExtensions.cs` | static class | `AddOrderingGrpcClients(services, configuration)`: registers both gRPC clients and their scoped `IGrpcApiClient` wrappers. |
| [`Order/`](Order/README.md) | folder | Client, DTO, requests/responses and mappers for `OrderService`. |
| [`OrderItem/`](OrderItem/README.md) | folder | Client, DTO, requests/responses and mappers for `OrderItemService`. |

## How it works

```csharp
services.AddGrpcClient<OrderService.OrderServiceClient>("OrderServiceClient.Ordering", (sp, options) =>
    options.Address = new Uri(sp.GetRequiredService<IOptions<CoreApiOptions>>().Value.BaseUrl));
services.AddScoped<Order.IGrpcApiClient, Order.GrpcApiClient>();
```

The same is done for `OrderItemService.OrderItemServiceClient` (name `OrderItemServiceClient.Ordering`) and `OrderItem.IGrpcApiClient`. Named clients use the `.Ordering` suffix so clients of other modules cannot collide.

## Dependencies
- References: `Acme.Core.Protocol.Prelaunch.Ordering.{Order,OrderItem}`, `CoreApiOptions`, `Grpc.Net.ClientFactory`.
- Used by: `AddCoreApiInfrastructure`; `Acme.Api.Ordering.*.Endpoints`.

## Extending
For a new Ordering resource add a resource folder (same layout as `Order/`), then register its `AddGrpcClient<...>` and `AddScoped<IGrpcApiClient, GrpcApiClient>` here.
