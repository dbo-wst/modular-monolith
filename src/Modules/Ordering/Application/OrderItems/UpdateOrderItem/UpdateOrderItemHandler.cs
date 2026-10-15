using Template.Core.Ordering.Domain.Orders;
using LanguageExt;
using Template.Core.Ordering.Domain.OrderItems;
using Template.Core.Ordering.Contracts.Commands;
using Template.Core.Ordering.Contracts.Results;
using Template.Core.Shared.Kernel.Communication;
using Template.Core.Ordering.Domain.ValueObjects;
using Template.Core.Shared.Kernel.Functional;

namespace Template.Core.Ordering.Application.OrderItems.UpdateOrderItem;

internal sealed class UpdateOrderItemHandler(Domain.OrderItems.IOrderItemRepository orderItems, Domain.Abstractions.IUnitOfWork unitOfWork): ICommandHandler<UpdateOrderItemCommand, UpdateOrderItemResult>
{
    public Task<Fin<UpdateOrderItemResult>> HandleAsync(UpdateOrderItemCommand request, CancellationToken ct = default) =>
        orderItems.GetByIdAsync(OrderItemId.From(request.OrderItemId), ct)
            .BindAsync(aggregate => aggregate.ChangeDetails(
                OrderId.From(request.OrderId),
                Quantity.From(request.Quantity),
                Price.From(request.Price)))
            .BindAsync(updated => orderItems.UpdateAsync(updated, ct))
            .BindAsync(updated => unitOfWork.SaveChangesAsync(ct).MapAsync(_ => updated))
            .MapAsync(updated => new UpdateOrderItemResult(DtoMappers.OrderItem.Map(updated)));
}

