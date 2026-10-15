using Grpc.Core;
using Acme.Core.BuildingBlocks.Application.Communication;
using GrpcProtocol = Acme.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Acme.Core.Api.Prelaunch.Ordering.OrderItem;

public sealed class GrpcService(IMediator mediator) : GrpcProtocol.OrderItemService.OrderItemServiceBase
{
    public override async Task<GrpcProtocol.CreateOrderItemResponse> CreateOrderItem(GrpcProtocol.CreateOrderItemRequest request, ServerCallContext context)
    {
        var result = await mediator.SendAsync(ProtoMappers.CreateOrderItemRequest.Map(request), context.CancellationToken);

        return result.Match(
            Succ: response => ProtoMappers.CreateOrderItemResponse.Map(response.OrderItem),
            Fail: ProtoMappers.CreateOrderItemErrorResponse.Map);
    }

    public override async Task<GrpcProtocol.GetOrderItemResponse> GetOrderItem(GrpcProtocol.GetOrderItemRequest request, ServerCallContext context)
    {
        var result = await mediator.SendAsync(ProtoMappers.GetOrderItemRequest.Map(request), context.CancellationToken);

        return result.Match(
            Succ: response => ProtoMappers.GetOrderItemResponse.Map(response.OrderItem),
            Fail: ProtoMappers.GetOrderItemErrorResponse.Map);
    }

    public override async Task<GrpcProtocol.GetOrderItemsResponse> GetOrderItems(GrpcProtocol.GetOrderItemsRequest request, ServerCallContext context)
    {
        var result = await mediator.SendAsync(ProtoMappers.GetOrderItemsRequest.Map(request), context.CancellationToken);

        return result.Match(
            Succ: response => ProtoMappers.GetOrderItemsResponse.Map((response.OrderItems, request.PageNumber, request.PageSize)),
            Fail: ProtoMappers.GetOrderItemsErrorResponse.Map);
    }

    public override async Task<GrpcProtocol.UpdateOrderItemResponse> UpdateOrderItem(GrpcProtocol.UpdateOrderItemRequest request, ServerCallContext context)
    {
        var result = await mediator.SendAsync(ProtoMappers.UpdateOrderItemRequest.Map(request), context.CancellationToken);

        return result.Match(
            Succ: response => ProtoMappers.UpdateOrderItemResponse.Map(response.OrderItem),
            Fail: ProtoMappers.UpdateOrderItemErrorResponse.Map);
    }

    public override async Task<GrpcProtocol.DeleteOrderItemResponse> DeleteOrderItem(GrpcProtocol.DeleteOrderItemRequest request, ServerCallContext context)
    {
        var result = await mediator.SendAsync(ProtoMappers.DeleteOrderItemRequest.Map(request), context.CancellationToken);

        return result.Match(
            Succ: _ => ProtoMappers.DeleteOrderItemResponse.Map(request.OrderItemId),
            Fail: ProtoMappers.DeleteOrderItemErrorResponse.Map);
    }
}
