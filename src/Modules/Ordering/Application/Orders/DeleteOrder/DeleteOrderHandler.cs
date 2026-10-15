using LanguageExt;
using Template.Core.Ordering.Domain.Orders;
using Template.Core.Ordering.Contracts.Commands;
using Template.Core.Ordering.Contracts.Results;
using Template.Core.Shared.Kernel.Communication;
using Template.Core.Shared.Kernel.Functional;

namespace Template.Core.Ordering.Application.Orders.DeleteOrder;

internal sealed class DeleteOrderHandler(Domain.Orders.IOrderRepository orders, Domain.Abstractions.IUnitOfWork unitOfWork): ICommandHandler<DeleteOrderCommand, DeleteOrderResult>
{

    public Task<Fin<DeleteOrderResult>> HandleAsync(DeleteOrderCommand request, CancellationToken ct = default)
    {
        var orderId = OrderId.From(request.OrderId);

        return orders.GetByIdAsync(orderId, ct)
            .BindAsync(order =>
                orders.DeleteAsync(orderId, ct)
                    .MapAsync(_ => { order.Delete(); return LanguageExt.Unit.Default; })
            )

            .BindAsync(_ => unitOfWork.SaveChangesAsync(ct))
            .MapAsync(_ => new DeleteOrderResult());
    }
}
