using LanguageExt;
using Wst.Trios.Core.BrokerSide.Domain.Order;
using Wst.Trios.Core.BrokerSide.Integration.Requests;
using Wst.Trios.Core.BrokerSide.Integration.Responses;
using Wst.Trios.Core.Shared.Kernel.Communication;
using Wst.Trios.Core.Shared.Kernel.Functional;

namespace Wst.Trios.Core.BrokerSide.Application.Order.Delete;

public class Service(Domain.IStore store): IRequestHandler<DeleteOrderRequest, DeleteOrderResponse>
{

    public Task<Fin<DeleteOrderResponse>> HandleAsync(DeleteOrderRequest request, CancellationToken ct = default)
    {
        var orderId = OrderId.From(request.OrderId);

        return store.Orders.GetByIdAsync(orderId, ct)
            .MapAsync(OrderAggregate.FromSnapshot)
            .BindAsync(order =>
                store.Orders.DeleteAsync(orderId, ct)
                    .BindAsync(i => order.Delete())
            )

            .BindAsync(_ => store.SaveChangesAsync(ct))
            .MapAsync(_ => new DeleteOrderResponse());
    }
}
