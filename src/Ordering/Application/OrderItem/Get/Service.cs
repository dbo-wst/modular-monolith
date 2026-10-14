using LanguageExt;
using Template.Core.Ordering.Domain;
using Template.Core.Ordering.Integration.Requests;
using Template.Core.Ordering.Integration.Responses;
using Template.Core.Shared.Kernel.Communication;
using Template.Core.Shared.Kernel.Functional;
using OrderItemId = Template.Core.Ordering.Domain.OrderItem.OrderItemId;

namespace Template.Core.Ordering.Application.OrderItem.Get;

public class Service(IStore store): IRequestHandler<GetOrderItemRequest, GetOrderItemResponse>
{
    public Task<Fin<GetOrderItemResponse>> HandleAsync(GetOrderItemRequest request, CancellationToken ct = default) =>
        store.OrderItems.GetByIdAsync(OrderItemId.From(request.OrderItemId), ct)
            .MapAsync(x => new GetOrderItemResponse(DtoMappers.OrderItem.Map(x)));
}
