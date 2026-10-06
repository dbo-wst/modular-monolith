using LanguageExt;
using Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Requests;
using Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Responses;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order;

public interface IGrpcApiClient
{
    Task<Fin<CreateOrderResponse>> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);

    Task<Fin<GetOrderResponse>> GetAsync(GetOrderRequest request, CancellationToken cancellationToken = default);

    Task<Fin<GetOrdersResponse>> ListAsync(GetOrdersRequest request, CancellationToken cancellationToken = default);

    Task<Fin<UpdateOrderResponse>> UpdateAsync(UpdateOrderRequest request, CancellationToken cancellationToken = default);

    Task<Fin<DeleteOrderResponse>> DeleteAsync(DeleteOrderRequest request, CancellationToken cancellationToken = default);
}
