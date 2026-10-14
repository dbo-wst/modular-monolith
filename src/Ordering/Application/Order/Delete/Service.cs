using LanguageExt;
using Template.Core.Ordering.Domain.Order;
using Template.Core.Ordering.Integration.Requests;
using Template.Core.Ordering.Integration.Responses;
using Template.Core.Shared.Kernel.Communication;
using Template.Core.Shared.Kernel.Functional;

namespace Template.Core.Ordering.Application.Order.Delete;

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
