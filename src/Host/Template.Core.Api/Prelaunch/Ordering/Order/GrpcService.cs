using Grpc.Core;
using Template.Core.BuildingBlocks.Application.Communication;
using GrpcProtocol = Template.Core.Protocol.Prelaunch.Ordering.Order;

namespace Template.Core.Api.Prelaunch.Ordering.Order;

public sealed class GrpcService(IMediator mediator) : GrpcProtocol.OrderService.OrderServiceBase
{
    public override async Task<GrpcProtocol.CreateOrderResponse> CreateOrder(GrpcProtocol.CreateOrderRequest request, ServerCallContext context)
    {
        var result = await mediator.SendAsync(ProtoMappers.CreateOrderRequest.Map(request), context.CancellationToken);

        return result.Match(
            Succ: response => ProtoMappers.CreateOrderResponse.Map(response.Order),
            Fail: ProtoMappers.CreateOrderErrorResponse.Map);
    }

    public override async Task<GrpcProtocol.GetOrderResponse> GetOrder(GrpcProtocol.GetOrderRequest request, ServerCallContext context)
    {
        var result = await mediator.SendAsync(ProtoMappers.GetOrderRequest.Map(request), context.CancellationToken);

        return result.Match(
            Succ: response => ProtoMappers.GetOrderResponse.Map(response.Order),
            Fail: ProtoMappers.GetOrderErrorResponse.Map);
    }

    public override async Task<GrpcProtocol.GetOrdersResponse> GetOrders(GrpcProtocol.GetOrdersRequest request, ServerCallContext context)
    {
        var result = await mediator.SendAsync(ProtoMappers.GetOrdersRequest.Map(request), context.CancellationToken);

        return result.Match(
            Succ: response => ProtoMappers.GetOrdersResponse.Map((response.Orders, request.PageNumber, request.PageSize)),
            Fail: ProtoMappers.GetOrdersErrorResponse.Map);
    }

    public override async Task<GrpcProtocol.UpdateOrderResponse> UpdateOrder(GrpcProtocol.UpdateOrderRequest request, ServerCallContext context)
    {
        var result = await mediator.SendAsync(ProtoMappers.UpdateOrderRequest.Map(request), context.CancellationToken);

        return result.Match(
            Succ: response => ProtoMappers.UpdateOrderResponse.Map(response.Order),
            Fail: ProtoMappers.UpdateOrderErrorResponse.Map);
    }

    public override async Task<GrpcProtocol.DeleteOrderResponse> DeleteOrder(GrpcProtocol.DeleteOrderRequest request, ServerCallContext context)
    {
        var result = await mediator.SendAsync(ProtoMappers.DeleteOrderRequest.Map(request), context.CancellationToken);

        return result.Match(
            Succ: _ => ProtoMappers.DeleteOrderResponse.Map(request.OrderId),
            Fail: ProtoMappers.DeleteOrderErrorResponse.Map);
    }
}
