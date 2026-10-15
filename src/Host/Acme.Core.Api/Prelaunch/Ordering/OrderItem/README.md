# OrderItem (gRPC service)

Server implementation of `OrderItemService`, namespace `Acme.Core.Api.Prelaunch.Ordering.OrderItem`. Translates Protocol messages to `Acme.Core.Ordering.Contracts` commands/queries, sends them through `IMediator` and maps the `Fin` result back to the response `oneof`.

## Contents

| Item | Kind | Description |
|---|---|---|
| `GrpcService.cs` | class | `GrpcService(IMediator) : OrderItemService.OrderItemServiceBase`; overrides `CreateOrderItem`, `GetOrderItem`, `GetOrderItems`, `UpdateOrderItem`, `DeleteOrderItem`. |
| `ProtoMappers.cs` | static class | Registry of mapper structs: requests, value (`OrderItem`), success responses, error responses. |
| `Mappers/` | folder | `OrderItem` (contract DTO `OrderItem` -> proto `OrderItem`) and per-operation folders `CreateOrderItem/`, `GetOrderItem/`, `GetOrderItems/`, `UpdateOrderItem/`, `DeleteOrderItem/` each with `Request.cs`, `Response.cs`, `ErrorResponse.cs`. |

## How it works

| RPC | Mediator request | Success branch |
|---|---|---|
| `CreateOrderItem` | `CreateOrderItemCommand(OrderId?, Quantity?, Price?)` | `success.order_item` |
| `GetOrderItem` | `GetOrderItemQuery(OrderItemId)` | `success.order_item` |
| `GetOrderItems` | `GetListOrderItemQuery(PageNumber, PageSize)` | `success.items` + echoed `page_number`/`page_size` |
| `UpdateOrderItem` | `UpdateOrderItemCommand(OrderItemId, OrderId?, Quantity?, Price?)` | `success.order_item` |
| `DeleteOrderItem` | `DeleteOrderItemCommand(OrderItemId)` | `deleted.order_item_id` (request id echoed) |

- Each method: `mediator.SendAsync(ProtoMappers.<Op>Request.Map(request), context.CancellationToken)` then `result.Match(Succ: ...Response.Map, Fail: ...ErrorResponse.Map)`.
- Request mappers: `bytes` -> `Guid` (`AsGuid`); `quantity`/`price` strings -> `decimal` via `decimal.Parse(..., CultureInfo.InvariantCulture)`; unset optional fields -> `null`.
- Value mapper `OrderItem`: `Guid` -> `bytes`; decimals -> invariant-culture strings; optional fields set only when present.
- Error mappers: `*Error { code = ErrorCodeClassifier.Classify(error), message = error.Message }`.

## Dependencies
- References: `Acme.Core.Protocol.Prelaunch.Ordering.OrderItem`, `Acme.Core.Ordering.Contracts` (Commands, Queries, Dto), `Acme.Core.BuildingBlocks.Application.Communication.IMediator`, `Acme.Core.BuildingBlocks.Domain` (`IOnewayMap`).
- Used by: `MapOrderingGrpcServices`; called by `Acme.Api.Infrastructure.CoreApi` `OrderItem.GrpcApiClient`.

## Extending
For a new RPC: declare it in `Protocol/Protos/prelaunch/ordering/order_item`, add `Mappers/<Op>/{Request,Response,ErrorResponse}.cs`, register them in `ProtoMappers`, and override the method in `GrpcService`.
