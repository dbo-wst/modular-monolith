using LanguageExt;
using Acme.Core.BuildingBlocks.Domain;
using Acme.Core.Ordering.Domain.OrderItems;
using Acme.Core.Ordering.Contracts.Commands;
using Acme.Core.Ordering.Contracts.Results;
using Acme.Core.BuildingBlocks.Application.Communication;
using Acme.Core.BuildingBlocks.Domain.Functional;

namespace Acme.Core.Ordering.Application.OrderItems.DeleteOrderItem;

internal sealed class DeleteOrderItemHandler(Domain.OrderItems.IOrderItemRepository orderItems, IUnitOfWork unitOfWork): ICommandHandler<DeleteOrderItemCommand, DeleteOrderItemResult>
{

    public Task<Fin<DeleteOrderItemResult>> HandleAsync(DeleteOrderItemCommand request, CancellationToken ct = default)
    {
        var orderItemId = OrderItemId.From(request.OrderItemId);

        return orderItems.GetByIdAsync(orderItemId, ct)
            .BindAsync(orderItem =>
            {
                // The repository tracks the aggregate, so its Deleted event is dispatched after a successful save.
                orderItem.Delete();
                return orderItems.DeleteAsync(orderItem, ct);
            })

            .BindAsync(_ => unitOfWork.SaveChangesAsync(ct))
            .MapAsync(_ => new DeleteOrderItemResult());
    }
}
