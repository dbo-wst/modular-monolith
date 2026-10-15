using LanguageExt;
using Acme.Core.BuildingBlocks.Domain;
using Acme.Core.Ordering.Domain.Orders;
using Acme.Core.Ordering.Contracts.Commands;
using Acme.Core.Ordering.Contracts.Results;
using Acme.Core.BuildingBlocks.Application.Communication;
using Acme.Core.BuildingBlocks.Domain.Functional;

namespace Acme.Core.Ordering.Application.Orders.DeleteOrder;

internal sealed class DeleteOrderHandler(Domain.Orders.IOrderRepository orders, IUnitOfWork unitOfWork): ICommandHandler<DeleteOrderCommand, DeleteOrderResult>
{

    public Task<Fin<DeleteOrderResult>> HandleAsync(DeleteOrderCommand request, CancellationToken ct = default)
    {
        var orderId = OrderId.From(request.OrderId);

        return orders.GetByIdAsync(orderId, ct)
            .BindAsync(order =>
            {
                // The repository tracks the aggregate, so its Deleted event is dispatched after a successful save.
                order.Delete();
                return orders.DeleteAsync(order, ct);
            })

            .BindAsync(_ => unitOfWork.SaveChangesAsync(ct))
            .MapAsync(_ => new DeleteOrderResult());
    }
}
