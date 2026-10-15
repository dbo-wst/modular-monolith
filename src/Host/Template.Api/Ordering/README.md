# Ordering (HTTP endpoints)

Gateway endpoints for the Ordering module, namespace `Template.Api.Ordering`. All routes live under the `/ordering` segment and delegate to the Ordering gRPC clients of `Template.Api.Infrastructure.CoreApi`.

## Contents

| Item | Kind | Description |
|---|---|---|
| `ServiceCollectionExtensions.cs` | static class | `MapOrderingEndpoints(this WebApplication)` calls `MapOrderEndpoints()` and `MapOrderItemEndpoints()`. |
| [`Order/`](Order/README.md) | folder | CRUD endpoints for orders at `/ordering/orders`. |
| [`OrderItem/`](OrderItem/README.md) | folder | CRUD endpoints for order items at `/ordering/order-items`. |

## How it works
- `Program.cs` calls `app.MapOrderingEndpoints()`; each resource registers its own route group with OpenAPI tags `Core.Ordering.Order` / `Core.Ordering.OrderItem`.
- Each endpoint resolves the resource-specific `IGrpcApiClient` from DI, awaits a `Fin<TResponse>` and matches it to an `IResult`.
- Failures are converted to ProblemDetails by a per-resource `ToProblem`/`StatusFor` pair (see resource READMEs).

## Dependencies
- References: `Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.*` (clients, requests, responses), `Template.Api.Segments`.
- Used by: `Program.cs` of `Template.Api`.

## Extending
Add a resource folder with an `Endpoints.cs` exposing `Map<Resource>Endpoints(this IEndpointRouteBuilder)` and call it from `MapOrderingEndpoints`.
