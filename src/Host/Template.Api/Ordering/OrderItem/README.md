# OrderItem (HTTP endpoints)

REST endpoints for order items, namespace `Template.Api.Ordering.OrderItem`. Each route forwards to `Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.IGrpcApiClient` and translates the result to HTTP.

## Contents

| Item | Kind | Description |
|---|---|---|
| `Endpoints.cs` | static class | `MapOrderItemEndpoints(this IEndpointRouteBuilder)` registers the `/ordering/order-items` group (tag `Core.Ordering.OrderItem`) plus `ToProblem`/`StatusFor` error translation. |

## How it works

| Method | Route | Name | Client call | Success | Problem codes |
|---|---|---|---|---|---|
| POST | `/ordering/order-items` | `Core.Ordering.CreateOrderItem` | `CreateAsync(CreateOrderItemRequest)` | 201 + `Location: /ordering/order-items/{orderItemId}`, `CreateOrderItemResponse` | 400, 409, 500 |
| GET | `/ordering/order-items/{id:guid}` | `Core.Ordering.GetOrderItem` | `GetAsync(new GetOrderItemRequest(id))` | 200 `GetOrderItemResponse` | 404, 500 |
| GET | `/ordering/order-items?pageNumber=1&pageSize=10` | `Core.Ordering.ListOrderItems` | `ListAsync(new GetOrderItemsRequest(pageNumber, pageSize))` | 200 `GetOrderItemsResponse` | 500 |
| PUT | `/ordering/order-items` | `Core.Ordering.UpdateOrderItem` | `UpdateAsync(UpdateOrderItemRequest)` (id in body) | 200 `UpdateOrderItemResponse` | 400, 404, 409, 500 |
| DELETE | `/ordering/order-items/{id:guid}` | `Core.Ordering.DeleteOrderItem` | `DeleteAsync(new DeleteOrderItemRequest(id))` | 204 | 404, 409, 500 |

Error translation is identical to the Order endpoints: `ApiError.ErrorCode` (or `internal` for any other `Error`) -> `not_found` 404, `validation` 400, `conflict`/`cancelled` 409, otherwise 500; returned as ProblemDetails with `title` = code and `detail` = message.

## Dependencies
- References: `Template.Api.Infrastructure.CoreApi` (`IGrpcApiClient`, `Requests`, `Responses`, `ApiError`), `Template.Core.BuildingBlocks.Domain.Errors.ErrorCodes`, `LanguageExt.Common.Error`.
- Used by: `Template.Api.Ordering.ServiceCollectionExtensions.MapOrderingEndpoints`.

## Extending
Add a new `group.MapXxx(...)` calling a new `IGrpcApiClient` method and match the result with `ToProblem`; keep `.WithName("Core.Ordering.<Op>")` and the `.Produces...` metadata in sync with the possible error codes.
