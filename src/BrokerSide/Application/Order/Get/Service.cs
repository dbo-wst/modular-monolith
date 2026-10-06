using LanguageExt;
using Wst.Trios.Core.BrokerSide.Domain;
using Wst.Trios.Core.BrokerSide.Integration.Requests;
using Wst.Trios.Core.BrokerSide.Integration.Responses;
using Wst.Trios.Core.Shared.Kernel.Communication;
using Wst.Trios.Core.Shared.Kernel.Functional;
using OrderId = Wst.Trios.Core.BrokerSide.Domain.Order.OrderId;

namespace Wst.Trios.Core.BrokerSide.Application.Order.Get;

public class Service(IStore store): IRequestHandler<GetOrderRequest, GetOrderResponse>
{
    public Task<Fin<GetOrderResponse>> HandleAsync(GetOrderRequest request, CancellationToken ct = default) =>
        store.Orders.GetByIdAsync(OrderId.From(request.OrderId), ct)
            .MapAsync(x => new GetOrderResponse(DtoMappers.Order.Map(x)));
}
