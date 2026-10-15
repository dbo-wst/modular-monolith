using LanguageExt;
using Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Requests;
using Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Responses;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order;

public interface IGrpcApiClient
{
    Task<Fin<CreateOrderResponse>> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);

    Task<Fin<GetOrderResponse>> GetAsync(GetOrderRequest request, CancellationToken cancellationToken = default);

    Task<Fin<GetOrdersResponse>> ListAsync(GetOrdersRequest request, CancellationToken cancellationToken = default);

    Task<Fin<UpdateOrderResponse>> UpdateAsync(UpdateOrderRequest request, CancellationToken cancellationToken = default);

    Task<Fin<DeleteOrderResponse>> DeleteAsync(DeleteOrderRequest request, CancellationToken cancellationToken = default);
}
