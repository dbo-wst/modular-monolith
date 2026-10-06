using LanguageExt;
using Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Requests;
using Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Responses;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order;

public sealed class GrpcApiClient(GrpcOrder.OrderService.OrderServiceClient client) : IGrpcApiClient
{
    public async Task<Fin<CreateOrderResponse>> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var response = await client.CreateOrderAsync(
            ProtoMappers.CreateOrderRequest.Map(request), cancellationToken: cancellationToken);

        return response.ResultCase == GrpcOrder.CreateOrderResponse.ResultOneofCase.Success
            ? Fin<CreateOrderResponse>.Succ(ProtoMappers.CreateOrderResponse.Map(response.Success))
            : Fin<CreateOrderResponse>.Fail(ProtoMappers.CreateOrderErrorResponse.Map(response.Error));
    }

    public async Task<Fin<GetOrderResponse>> GetAsync(GetOrderRequest request, CancellationToken cancellationToken = default)
    {
        var response = await client.GetOrderAsync(
            ProtoMappers.GetOrderRequest.Map(request), cancellationToken: cancellationToken);

        return response.ResultCase == GrpcOrder.GetOrderResponse.ResultOneofCase.Success
            ? Fin<GetOrderResponse>.Succ(ProtoMappers.GetOrderResponse.Map(response.Success))
            : Fin<GetOrderResponse>.Fail(ProtoMappers.GetOrderErrorResponse.Map(response.Error));
    }

    public async Task<Fin<GetOrdersResponse>> ListAsync(GetOrdersRequest request, CancellationToken cancellationToken = default)
    {
        var response = await client.GetOrdersAsync(
            ProtoMappers.GetOrdersRequest.Map(request), cancellationToken: cancellationToken);

        return response.ResultCase == GrpcOrder.GetOrdersResponse.ResultOneofCase.Success
            ? Fin<GetOrdersResponse>.Succ(ProtoMappers.GetOrdersResponse.Map(response.Success))
            : Fin<GetOrdersResponse>.Fail(ProtoMappers.GetOrdersErrorResponse.Map(response.Error));
    }

    public async Task<Fin<UpdateOrderResponse>> UpdateAsync(UpdateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var response = await client.UpdateOrderAsync(
            ProtoMappers.UpdateOrderRequest.Map(request), cancellationToken: cancellationToken);

        return response.ResultCase == GrpcOrder.UpdateOrderResponse.ResultOneofCase.Success
            ? Fin<UpdateOrderResponse>.Succ(ProtoMappers.UpdateOrderResponse.Map(response.Success))
            : Fin<UpdateOrderResponse>.Fail(ProtoMappers.UpdateOrderErrorResponse.Map(response.Error));
    }

    public async Task<Fin<DeleteOrderResponse>> DeleteAsync(DeleteOrderRequest request, CancellationToken cancellationToken = default)
    {
        var response = await client.DeleteOrderAsync(
            ProtoMappers.DeleteOrderRequest.Map(request), cancellationToken: cancellationToken);

        return response.ResultCase == GrpcOrder.DeleteOrderResponse.ResultOneofCase.Deleted
            ? Fin<DeleteOrderResponse>.Succ(ProtoMappers.DeleteOrderResponse.Map(response.Deleted))
            : Fin<DeleteOrderResponse>.Fail(ProtoMappers.DeleteOrderErrorResponse.Map(response.Error));
    }
}
