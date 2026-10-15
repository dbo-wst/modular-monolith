# Order (gateway gRPC client)

Typed client for `OrderService`, namespace `Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order`. Converts client request records to Protocol messages, calls Core.Api and returns `Fin<TResponse>` with `ApiError` on failure.

## Contents

| Item | Kind | Description |
|---|---|---|
| `IGrpcApiClient.cs` | interface | `CreateAsync`, `GetAsync`, `ListAsync`, `UpdateAsync`, `DeleteAsync`, each returning `Task<Fin<...Response>>`. |
| `GrpcApiClient.cs` | class | Implementation over `OrderService.OrderServiceClient`; checks `ResultCase` (`Success`, or `Deleted` for delete) and maps either branch. |
| `OrderDto.cs` | record | Client model `OrderDto(Guid OrderId, string? Number, DateOnly? EffectiveDate, EStatus? Status)`. |
| `EStatus.cs` | enum | Client status enum with JSON names `UNSPECIFIED`, `NEW`, `IN_PROGRESS`, `COMPLETED`, `CANCELLED`, `REJECTED`. |
| `ProtoMappers.cs` | static class | Registry of mapper structs: requests, values (`Order`, `Status`), success responses, error responses. |
| `Requests/` | folder | `CreateOrderRequest(Number?, EffectiveDate?)`, `GetOrderRequest(OrderId)`, `GetOrdersRequest(PageNumber, PageSize)`, `UpdateOrderRequest(OrderId, Number?, EffectiveDate?, Status?)` (null = not updated), `DeleteOrderRequest(OrderId)`. |
| `Responses/` | folder | `CreateOrderResponse(Order)`, `GetOrderResponse(Order)`, `GetOrdersResponse(Orders)`, `UpdateOrderResponse(Order)`, empty `DeleteOrderResponse`. |
| `Mappers/` | folder | `Order` (proto `Order` -> `OrderDto`), `StatusMapper` (`EStatus` <-> proto `Status`), internal `ApiErrors.From(code, message)`, and per-operation folders `CreateOrder/`, `GetOrder/`, `GetOrders/`, `UpdateOrder/`, `DeleteOrder/` each with `Request.cs`, `Response.cs`, `ErrorResponse.cs`. |

## How it works
1. Request mapper builds the proto message: `Guid` -> `bytes` via `AsByteString()`, `DateOnly` -> `Timestamp` via `AsTimestamp()`, optional fields set only when non-null, `Status` via `StatusMapper`.
2. `GrpcApiClient` calls e.g. `client.CreateOrderAsync(...)` with the cancellation token.
3. On success: `*Success` -> response record (`Order` mapper: `bytes` -> `Guid`, `Timestamp` -> `DateOnly`, `HasStatus` -> `EStatus`). `GetOrders` drops `page_number`/`page_size` and returns only the list.
4. On error: `*ErrorResponse` mapper -> `ApiErrors.From(error.Code, error.Message)`; a null error becomes `ApiError(internal, "The Core API returned an empty error.")`.

`StatusMapper` maps proto `STATUS_UNSPECIFIED` to `EStatus.Unspecified`, but throws `ArgumentOutOfRangeException` when mapping `EStatus.Unspecified` to proto.

## Dependencies
- References: `Template.Core.Protocol` (`OrderService`, messages, `BytesExtension`, `TimestampExtension`), `Template.Core.BuildingBlocks.Domain` (`IMap`, `IOnewayMap`, `ErrorCodes`), `LanguageExt` (`Fin`), `ApiError`.
- Used by: `Template.Api.Ordering.Order.Endpoints`; registered in `Prelaunch/Ordering/ServiceCollectionExtensions.cs`.

## Extending
For a new RPC: add request/response records, a `Mappers/<Op>/` folder with `Request`, `Response`, `ErrorResponse` structs, register them as fields in `ProtoMappers`, and add the method to `IGrpcApiClient`/`GrpcApiClient`.
