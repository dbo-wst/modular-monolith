using LanguageExt;
using Template.Core.BuildingBlocks.Domain;
using Template.Core.Ordering.Domain.Orders;
using Template.Core.Ordering.Contracts.Commands;
using Template.Core.Ordering.Contracts.Results;
using Template.Core.BuildingBlocks.Application.Communication;
using Template.Core.BuildingBlocks.Domain.Functional;

namespace Template.Core.Ordering.Application.Orders.DeleteOrder;

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
