using Acme.Core.Ordering.Domain.Orders;
using LanguageExt;
using Acme.Core.BuildingBlocks.Domain;
using Acme.Core.Ordering.Domain.OrderItems;
using Acme.Core.Ordering.Contracts.Commands;
using Acme.Core.Ordering.Contracts.Results;
using Acme.Core.BuildingBlocks.Application.Communication;
using Acme.Core.Ordering.Domain.ValueObjects;
using Acme.Core.BuildingBlocks.Domain.Functional;

namespace Acme.Core.Ordering.Application.OrderItems.UpdateOrderItem;

internal sealed class UpdateOrderItemHandler(Domain.OrderItems.IOrderItemRepository orderItems, IUnitOfWork unitOfWork): ICommandHandler<UpdateOrderItemCommand, UpdateOrderItemResult>
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

