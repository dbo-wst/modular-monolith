using LanguageExt;

using Wst.Trios.Core.BrokerSide.Domain.Order;
using Wst.Trios.Core.BrokerSide.Integration.Requests;
using Wst.Trios.Core.BrokerSide.Integration.Responses;
using Wst.Trios.Core.Shared.Kernel.Communication;
using Wst.Trios.Core.Shared.Kernel.Domain;
using Wst.Trios.Core.Shared.Kernel.Functional;

namespace Wst.Trios.Core.BrokerSide.Application.Order.Create;

public class Service(Domain.IStore store) : IRequestHandler<CreateOrderRequest, CreateOrderResponse>
{
    public Task<Fin<CreateOrderResponse>> HandleAsync(CreateOrderRequest request, CancellationToken ct = default) =>
        OrderAggregate.Create(
                BrokerId.From(request.BrokerId),
                InstrumentId.From(request.InstrumentId),
                VenueId.From(request.VenueId),
                OrderNumber.From(request.Number),
                EffectiveDate.From(request.EffectiveDate),
                Quantity.From(request.Quantity),
                Price.From(request.Price),
                Side.From(request.Side != null ? DtoMappers.Side.Map(request.Side.Value) : null))
            .AsTask()
            .MapAsync(i =>i.ToSnapshot())
            .BindAsync(order => store.Orders.AddAsync(order, ct))
            .BindAsync(created => store.SaveChangesAsync(ct).MapAsync(_ => created))
            .MapAsync(created => new CreateOrderResponse(DtoMappers.Order.Map(created)));
}
