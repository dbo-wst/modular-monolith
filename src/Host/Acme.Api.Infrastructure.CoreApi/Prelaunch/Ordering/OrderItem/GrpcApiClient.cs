using LanguageExt;
using Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Requests;
using Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Responses;
using GrpcOrderItem = Acme.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem;

public sealed class GrpcApiClient(GrpcOrderItem.OrderItemService.OrderItemServiceClient client) : IGrpcApiClient
{
    public async Task<Fin<CreateOrderItemResponse>> CreateAsync(CreateOrderItemRequest request, CancellationToken cancellationToken = default)
    {
        var response = await client.CreateOrderItemAsync(
            ProtoMappers.CreateOrderItemRequest.Map(request), cancellationToken: cancellationToken);

        return response.ResultCase == GrpcOrderItem.CreateOrderItemResponse.ResultOneofCase.Success
            ? Fin<CreateOrderItemResponse>.Succ(ProtoMappers.CreateOrderItemResponse.Map(response.Success))
            : Fin<CreateOrderItemResponse>.Fail(ProtoMappers.CreateOrderItemErrorResponse.Map(response.Error));
    }

    public async Task<Fin<GetOrderItemResponse>> GetAsync(GetOrderItemRequest request, CancellationToken cancellationToken = default)
    {
        var response = await client.GetOrderItemAsync(
            ProtoMappers.GetOrderItemRequest.Map(request), cancellationToken: cancellationToken);

        return response.ResultCase == GrpcOrderItem.GetOrderItemResponse.ResultOneofCase.Success
            ? Fin<GetOrderItemResponse>.Succ(ProtoMappers.GetOrderItemResponse.Map(response.Success))
            : Fin<GetOrderItemResponse>.Fail(ProtoMappers.GetOrderItemErrorResponse.Map(response.Error));
    }

    public async Task<Fin<GetOrderItemsResponse>> ListAsync(GetOrderItemsRequest request, CancellationToken cancellationToken = default)
    {
        var response = await client.GetOrderItemsAsync(
            ProtoMappers.GetOrderItemsRequest.Map(request), cancellationToken: cancellationToken);

        return response.ResultCase == GrpcOrderItem.GetOrderItemsResponse.ResultOneofCase.Success
            ? Fin<GetOrderItemsResponse>.Succ(ProtoMappers.GetOrderItemsResponse.Map(response.Success))
            : Fin<GetOrderItemsResponse>.Fail(ProtoMappers.GetOrderItemsErrorResponse.Map(response.Error));
    }

    public async Task<Fin<UpdateOrderItemResponse>> UpdateAsync(UpdateOrderItemRequest request, CancellationToken cancellationToken = default)
    {
        var response = await client.UpdateOrderItemAsync(
            ProtoMappers.UpdateOrderItemRequest.Map(request), cancellationToken: cancellationToken);

        return response.ResultCase == GrpcOrderItem.UpdateOrderItemResponse.ResultOneofCase.Success
            ? Fin<UpdateOrderItemResponse>.Succ(ProtoMappers.UpdateOrderItemResponse.Map(response.Success))
            : Fin<UpdateOrderItemResponse>.Fail(ProtoMappers.UpdateOrderItemErrorResponse.Map(response.Error));
    }

    public async Task<Fin<DeleteOrderItemResponse>> DeleteAsync(DeleteOrderItemRequest request, CancellationToken cancellationToken = default)
    {
        var response = await client.DeleteOrderItemAsync(
            ProtoMappers.DeleteOrderItemRequest.Map(request), cancellationToken: cancellationToken);

        return response.ResultCase == GrpcOrderItem.DeleteOrderItemResponse.ResultOneofCase.Deleted
            ? Fin<DeleteOrderItemResponse>.Succ(ProtoMappers.DeleteOrderItemResponse.Map(response.Deleted))
            : Fin<DeleteOrderItemResponse>.Fail(ProtoMappers.DeleteOrderItemErrorResponse.Map(response.Error));
    }
}
