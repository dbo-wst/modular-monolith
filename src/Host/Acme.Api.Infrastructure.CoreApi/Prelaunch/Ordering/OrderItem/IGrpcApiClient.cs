using LanguageExt;
using Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Requests;
using Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Responses;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem;

public interface IGrpcApiClient
{
    Task<Fin<CreateOrderItemResponse>> CreateAsync(CreateOrderItemRequest request, CancellationToken cancellationToken = default);

    Task<Fin<GetOrderItemResponse>> GetAsync(GetOrderItemRequest request, CancellationToken cancellationToken = default);

    Task<Fin<GetOrderItemsResponse>> ListAsync(GetOrderItemsRequest request, CancellationToken cancellationToken = default);

    Task<Fin<UpdateOrderItemResponse>> UpdateAsync(UpdateOrderItemRequest request, CancellationToken cancellationToken = default);

    Task<Fin<DeleteOrderItemResponse>> DeleteAsync(DeleteOrderItemRequest request, CancellationToken cancellationToken = default);
}
