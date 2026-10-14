using LanguageExt;

using Template.Core.Ordering.Domain.Order;
using Template.Core.Ordering.Integration.Requests;
using Template.Core.Ordering.Integration.Responses;
using Template.Core.Shared.Kernel.Communication;
using Template.Core.Shared.Kernel.Domain;
using Template.Core.Shared.Kernel.Functional;

namespace Template.Core.Ordering.Application.Order.Create;

public class Service(Domain.IStore store) : IRequestHandler<CreateOrderRequest, CreateOrderResponse>
{
    public Task<Fin<CreateOrderResponse>> HandleAsync(CreateOrderRequest request, CancellationToken ct = default) =>
        OrderAggregate.Create(
                OrderNumber.From(request.Number),
                EffectiveDate.From(request.EffectiveDate))
            .AsTask()
            .MapAsync(i =>i.ToSnapshot())
            .BindAsync(order => store.Orders.AddAsync(order, ct))
            .BindAsync(created => store.SaveChangesAsync(ct).MapAsync(_ => created))
            .MapAsync(created => new CreateOrderResponse(DtoMappers.Order.Map(created)));
}
