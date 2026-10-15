# Order (HTTP endpoints)

REST endpoints for orders, namespace `Acme.Api.Ordering.Order`. Each route forwards to `Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.IGrpcApiClient` and translates the result to HTTP.

## Contents

| Item | Kind | Description |
|---|---|---|
| `Endpoints.cs` | static class | `MapOrderEndpoints(this IEndpointRouteBuilder)` registers the `/ordering/orders` group (tag `Core.Ordering.Order`) plus `ToProblem`/`StatusFor` error translation. |

## How it works

| Method | Route | Name | Client call | Success | Problem codes |
|---|---|---|---|---|---|
| POST | `/ordering/orders` | `Core.Ordering.CreateOrder` | `CreateAsync(CreateOrderRequest)` | 201 + `Location: /ordering/orders/{orderId}`, `CreateOrderResponse` | 400, 409, 500 |
| GET | `/ordering/orders/{id:guid}` | `Core.Ordering.GetOrder` | `GetAsync(new GetOrderRequest(id))` | 200 `GetOrderResponse` | 404, 500 |
| GET | `/ordering/orders?pageNumber=1&pageSize=10` | `Core.Ordering.ListOrders` | `ListAsync(new GetOrdersRequest(pageNumber, pageSize))` | 200 `GetOrdersResponse` | 500 |
| PUT | `/ordering/orders` | `Core.Ordering.UpdateOrder` | `UpdateAsync(UpdateOrderRequest)` (id in body) | 200 `UpdateOrderResponse` | 400, 404, 409, 500 |
| DELETE | `/ordering/orders/{id:guid}` | `Core.Ordering.DeleteOrder` | `DeleteAsync(new DeleteOrderRequest(id))` | 204 | 404, 409, 500 |

Error translation: `ToProblem` takes `ApiError.ErrorCode` (any other `Error` becomes `ErrorCodes.Internal`) and returns `Results.Problem(detail: message, title: code, statusCode: StatusFor(code))`:

| `ErrorCodes` | HTTP |
|---|---|
| `not_found` | 404 |
| `validation` | 400 |
| `conflict`, `cancelled` | 409 |
| anything else (`internal`) | 500 |

`pageNumber`/`pageSize` default to 1 and 10. `Status` is serialized as string enum values (`NEW`, `IN_PROGRESS`, ...).

## Dependencies
- References: `Acme.Api.Infrastructure.CoreApi` (`IGrpcApiClient`, `Requests`, `Responses`, `ApiError`), `Acme.Core.BuildingBlocks.Domain.Errors.ErrorCodes`, `LanguageExt.Common.Error`.
- Used by: `Acme.Api.Ordering.ServiceCollectionExtensions.MapOrderingEndpoints`.

## Extending
Add a new `group.MapXxx(...)` that calls a new `IGrpcApiClient` method and matches with `ToProblem`; declare `.WithName("Core.Ordering.<Op>")`, `.Produces<...>()` and the possible `.ProducesProblem(...)` codes.
