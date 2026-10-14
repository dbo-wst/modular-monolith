using LanguageExt;
using Template.Core.Ordering.Domain.Order;
using Template.Core.Ordering.Integration.Requests;
using Template.Core.Ordering.Integration.Responses;
using Template.Core.Shared.Kernel.Communication;
using Template.Core.Shared.Kernel.Domain;
using Template.Core.Shared.Kernel.Functional;

namespace Template.Core.Ordering.Application.Order.Update;

public class Service(Domain.IStore store): IRequestHandler<UpdateOrderRequest, UpdateOrderResponse>
{
    public Task<Fin<UpdateOrderResponse>> HandleAsync(UpdateOrderRequest request, CancellationToken ct = default) =>
        store.Orders.GetByIdAsync(OrderId.From(request.OrderId), ct)
            .MapAsync(OrderAggregate.FromSnapshot)
            .BindAsync(aggregate => aggregate.ChangeDetails(
                OrderNumber.From(request.Number),
                EffectiveDate.From(request.EffectiveDate),
                Status.From(request.Status != null ? DtoMappers.Status.Map(request.Status.Value) : null)
                ))
            .MapAsync(agg => agg.ToSnapshot())
            .BindAsync(snapshot => store.Orders.UpdateAsync(snapshot, ct))
            .BindAsync(snapshot => store.SaveChangesAsync(ct).MapAsync(_ => snapshot))
            .MapAsync(snapshot => new UpdateOrderResponse(DtoMappers.Order.Map(snapshot)));
}
