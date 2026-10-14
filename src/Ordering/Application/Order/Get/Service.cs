using LanguageExt;
using Template.Core.Ordering.Domain;
using Template.Core.Ordering.Integration.Requests;
using Template.Core.Ordering.Integration.Responses;
using Template.Core.Shared.Kernel.Communication;
using Template.Core.Shared.Kernel.Functional;
using OrderId = Template.Core.Ordering.Domain.Order.OrderId;

namespace Template.Core.Ordering.Application.Order.Get;

public class Service(IStore store): IRequestHandler<GetOrderRequest, GetOrderResponse>
{
    public Task<Fin<GetOrderResponse>> HandleAsync(GetOrderRequest request, CancellationToken ct = default) =>
        store.Orders.GetByIdAsync(OrderId.From(request.OrderId), ct)
            .MapAsync(x => new GetOrderResponse(DtoMappers.Order.Map(x)));
}
