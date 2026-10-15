using LanguageExt;
using Template.Core.Ordering.Domain.OrderItems;
using Template.Core.Ordering.Contracts.Commands;
using Template.Core.Ordering.Contracts.Results;
using Template.Core.Shared.Kernel.Communication;
using Template.Core.Shared.Kernel.Functional;

namespace Template.Core.Ordering.Application.OrderItems.DeleteOrderItem;

internal sealed class DeleteOrderItemHandler(Domain.OrderItems.IOrderItemRepository orderItems, Domain.Abstractions.IUnitOfWork unitOfWork): ICommandHandler<DeleteOrderItemCommand, DeleteOrderItemResult>
{

    public Task<Fin<DeleteOrderItemResult>> HandleAsync(DeleteOrderItemCommand request, CancellationToken ct = default)
    {
        var orderItemId = OrderItemId.From(request.OrderItemId);

        return orderItems.GetByIdAsync(orderItemId, ct)
            .BindAsync(orderItem =>
                orderItems.DeleteAsync(orderItemId, ct)
                    .MapAsync(_ => { orderItem.Delete(); return LanguageExt.Unit.Default; })
            )

            .BindAsync(_ => unitOfWork.SaveChangesAsync(ct))
            .MapAsync(_ => new DeleteOrderItemResult());
    }
}
