using LanguageExt;
using Template.Core.BuildingBlocks.Domain;
using Template.Core.Ordering.Domain.OrderItems;
using Template.Core.Ordering.Contracts.Commands;
using Template.Core.Ordering.Contracts.Results;
using Template.Core.BuildingBlocks.Application.Communication;
using Template.Core.BuildingBlocks.Domain.Functional;

namespace Template.Core.Ordering.Application.OrderItems.DeleteOrderItem;

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
