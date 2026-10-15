# OrderItem (gateway gRPC client)

Typed client for `OrderItemService`, namespace `Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem`. Converts client request records to Protocol messages, calls Core.Api and returns `Fin<TResponse>` with `ApiError` on failure.

## Contents

| Item | Kind | Description |
|---|---|---|
| `IGrpcApiClient.cs` | interface | `CreateAsync`, `GetAsync`, `ListAsync`, `UpdateAsync`, `DeleteAsync`, each returning `Task<Fin<...Response>>`. |
| `GrpcApiClient.cs` | class | Implementation over `OrderItemService.OrderItemServiceClient`; checks `ResultCase` (`Success`, or `Deleted` for delete) and maps either branch. |
| `OrderItemDto.cs` | record | Client model `OrderItemDto(Guid OrderItemId, Guid? OrderId, decimal? Quantity, decimal? Price)`. |
| `ProtoMappers.cs` | static class | Registry of mapper structs: requests, value (`OrderItem`), success responses, error responses. |
| `Requests/` | folder | `CreateOrderItemRequest(OrderId?, Quantity?, Price?)`, `GetOrderItemRequest(OrderItemId)`, `GetOrderItemsRequest(PageNumber, PageSize)`, `UpdateOrderItemRequest(OrderItemId, OrderId?, Quantity?, Price?)` (null = not updated), `DeleteOrderItemRequest(OrderItemId)`. |
| `Responses/` | folder | `CreateOrderItemResponse(OrderItem)`, `GetOrderItemResponse(OrderItem)`, `GetOrderItemsResponse(OrderItems)`, `UpdateOrderItemResponse(OrderItem)`, empty `DeleteOrderItemResponse`. |
| `Mappers/` | folder | `OrderItem` (proto `OrderItem` -> `OrderItemDto`), internal `ApiErrors.From(code, message)`, and per-operation folders `CreateOrderItem/`, `GetOrderItem/`, `GetOrderItems/`, `UpdateOrderItem/`, `DeleteOrderItem/` each with `Request.cs`, `Response.cs`, `ErrorResponse.cs`. |

## How it works
1. Request mapper builds the proto message: `Guid` -> `bytes` via `AsByteString()`; `decimal` -> `string` with `CultureInfo.InvariantCulture`; optional fields set only when non-null.
2. `GrpcApiClient` calls e.g. `client.CreateOrderItemAsync(...)` with the cancellation token.
3. On success: `*Success` -> response record (`OrderItem` mapper parses `quantity`/`price` with `decimal.Parse(..., InvariantCulture)`). `GetOrderItems` maps proto `items` and drops paging fields.
4. On error: `*ErrorResponse` mapper -> `ApiErrors.From(error.Code, error.Message)`; a null error becomes `ApiError(internal, "The Core API returned an empty error.")`.

## Dependencies
- References: `Acme.Core.Protocol` (`OrderItemService`, messages, `BytesExtension`), `Acme.Core.BuildingBlocks.Domain` (`IOnewayMap`, `ErrorCodes`), `LanguageExt` (`Fin`), `ApiError`.
- Used by: `Acme.Api.Ordering.OrderItem.Endpoints`; registered in `Prelaunch/Ordering/ServiceCollectionExtensions.cs`.

## Extending
For a new RPC: add request/response records, a `Mappers/<Op>/` folder with `Request`, `Response`, `ErrorResponse` structs, register them in `ProtoMappers`, and add the method to `IGrpcApiClient`/`GrpcApiClient`.
