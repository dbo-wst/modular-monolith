# Ordering (gRPC services)

Host-side wiring of the Ordering module, namespace `Acme.Core.Api.Prelaunch.Ordering`. Registers the module's Application and Persistence layers and maps its gRPC services.

## Contents

| Item | Kind | Description |
|---|---|---|
| `ServiceCollectionExtensions.cs` | static class | `AddOrderingModule(services, configuration)` and `MapOrderingGrpcServices(app)`. |
| [`Order/`](Order/README.md) | folder | `GrpcService` implementing `OrderService` plus mappers. |
| [`OrderItem/`](OrderItem/README.md) | folder | `GrpcService` implementing `OrderItemService` plus mappers. |

## How it works

```csharp
public static IServiceCollection AddOrderingModule(this IServiceCollection services, IConfiguration configuration)
{
    services.AddOrderingApplication();             // handlers + validators of Acme.Core.Ordering.Application
    services.AddOrderingPersistence(configuration); // Persistence:ConnectionString, DbContext, unit of work, repositories
    return services;
}

public static void MapOrderingGrpcServices(this WebApplication app)
{
    app.MapGrpcService<Order.GrpcService>();
    app.MapGrpcService<OrderItem.GrpcService>();
}
```

The mediator itself is not registered here; `Program.cs` calls `AddMediator()` once for all modules.

## Dependencies
- References: `Acme.Core.Ordering.Application`, `Acme.Core.Ordering.Persistence`.
- Used by: `Acme.Core.Api/Program.cs`.

## Extending
For a new Ordering resource add a resource folder with a `GrpcService` (same layout as `Order/`) and a `MapGrpcService<...>` line in `MapOrderingGrpcServices`.
