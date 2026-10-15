# Order (gRPC service)

Server implementation of `OrderService`, namespace `Template.Core.Api.Prelaunch.Ordering.Order`. Translates Protocol messages to `Template.Core.Ordering.Contracts` commands/queries, sends them through `IMediator` and maps the `Fin` result back to the response `oneof`.

## Contents

| Item | Kind | Description |
|---|---|---|
| `GrpcService.cs` | class | `GrpcService(IMediator) : OrderService.OrderServiceBase`; overrides `CreateOrder`, `GetOrder`, `GetOrders`, `UpdateOrder`, `DeleteOrder`. |
| `ProtoMappers.cs` | static class | Registry of mapper structs: requests, values (`Order`, `Status`), success responses, error responses. |
| `Mappers/` | folder | `Order` (contract DTO `Order` -> proto `Order`), `StatusMapper` (contract `EStatus` <-> proto `Status`), and per-operation folders `CreateOrder/`, `GetOrder/`, `GetOrders/`, `UpdateOrder/`, `DeleteOrder/` each with `Request.cs`, `Response.cs`, `ErrorResponse.cs`. |

## How it works

| RPC | Mediator request | Success branch |
|---|---|---|
| `CreateOrder` | `CreateOrderCommand(Number?, EffectiveDate?)` | `success.order` |
| `GetOrder` | `GetOrderQuery(OrderId)` | `success.order` |
| `GetOrders` | `GetListOrderQuery(PageNumber, PageSize)` | `success.orders` + echoed `page_number`/`page_size` |
| `UpdateOrder` | `UpdateOrderCommand(OrderId, Number?, EffectiveDate?, Status?)` | `success.order` |
| `DeleteOrder` | `DeleteOrderCommand(OrderId)` | `deleted.order_id` (request id echoed) |

Each method follows the same shape:

```csharp
var result = await mediator.SendAsync(ProtoMappers.CreateOrderRequest.Map(request), context.CancellationToken);
return result.Match(Succ: r => ProtoMappers.CreateOrderResponse.Map(r.Order), Fail: ProtoMappers.CreateOrderErrorResponse.Map);
```

- Request mappers: `bytes` -> `Guid` (`AsGuid`), `Timestamp` -> `DateOnly` (`AsDateOnly`), `HasNumber`/`HasStatus` -> nullable; `STATUS_UNSPECIFIED` on update means "not changed". `CreateOrderRequest.order_id` is not read.
- Value mapper `Order`: sets `number`/`effective_date` only when present; null status -> `STATUS_UNSPECIFIED`.
- Error mappers: `*Error { code = ErrorCodeClassifier.Classify(error), message = error.Message }`. No gRPC status exceptions are thrown for domain failures.

## Dependencies
- References: `Template.Core.Protocol.Prelaunch.Ordering.Order`, `Template.Core.Ordering.Contracts` (Commands, Queries, Dto), `Template.Core.BuildingBlocks.Application.Communication.IMediator`, `Template.Core.BuildingBlocks.Domain` (`IMap`, `IOnewayMap`).
- Used by: `MapOrderingGrpcServices`; called by `Template.Api.Infrastructure.CoreApi` `Order.GrpcApiClient`.

## Extending
For a new RPC: declare it in `Protocol/Protos/prelaunch/ordering/order`, add `Mappers/<Op>/{Request,Response,ErrorResponse}.cs`, register them in `ProtoMappers`, and override the method in `GrpcService` using the pattern above.
