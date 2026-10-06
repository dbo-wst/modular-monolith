using LanguageExt;
using Wst.Trios.Core.BrokerSide.Domain.Order;
using Wst.Trios.Core.BrokerSide.Integration.Requests;
using Wst.Trios.Core.BrokerSide.Integration.Responses;
using Wst.Trios.Core.Shared.Kernel.Communication;
using Wst.Trios.Core.Shared.Kernel.Domain;
using Wst.Trios.Core.Shared.Kernel.Functional;

namespace Wst.Trios.Core.BrokerSide.Application.Order.Update;

public class Service(Domain.IStore store): IRequestHandler<UpdateOrderRequest, UpdateOrderResponse>
{
    public Task<Fin<UpdateOrderResponse>> HandleAsync(UpdateOrderRequest request, CancellationToken ct = default) =>
        store.Orders.GetByIdAsync(OrderId.From(request.OrderId), ct)
            .MapAsync(OrderAggregate.FromSnapshot)
            .BindAsync(aggregate => aggregate.ChangeDetails(
                BrokerId.From(request.BrokerId),
                InstrumentId.From(request.InstrumentId),
                VenueId.From(request.VenueId),
                OrderNumber.From(request.Number),
                EffectiveDate.From(request.EffectiveDate),
                Quantity.From(request.Quantity),
                Price.From(request.Price),
                Side.From(request.Side != null ? DtoMappers.Side.Map(request.Side.Value) : null),
                Status.From(request.Status != null ? DtoMappers.Status.Map(request.Status.Value) : null)
                ))
            .MapAsync(agg => agg.ToSnapshot())
            .BindAsync(snapshot => store.Orders.UpdateAsync(snapshot, ct))
            .BindAsync(snapshot => store.SaveChangesAsync(ct).MapAsync(_ => snapshot))
            .MapAsync(snapshot => new UpdateOrderResponse(DtoMappers.Order.Map(snapshot)));
}
