using LanguageExt;
using Template.Core.Ordering.Domain.Orders;
using Template.Core.Ordering.Contracts.Commands;
using Template.Core.Ordering.Contracts.Results;
using Template.Core.Shared.Kernel.Communication;
using Template.Core.Ordering.Domain.ValueObjects;
using Template.Core.Shared.Kernel.Errors;
using Template.Core.Shared.Kernel.Functional;

namespace Template.Core.Ordering.Application.Orders.UpdateOrder;

internal sealed class UpdateOrderHandler(Domain.Orders.IOrderRepository orders, Domain.Abstractions.IUnitOfWork unitOfWork): ICommandHandler<UpdateOrderCommand, UpdateOrderResult>
{
    public Task<Fin<UpdateOrderResult>> HandleAsync(UpdateOrderCommand request, CancellationToken ct = default) =>
        orders.GetByIdAsync(OrderId.From(request.OrderId), ct)
            .BindAsync(aggregate => Apply(aggregate, request))
            .BindAsync(updated => orders.UpdateAsync(updated, ct))
            .BindAsync(updated => unitOfWork.SaveChangesAsync(ct).MapAsync(_ => updated))
            .MapAsync(updated => new UpdateOrderResult(DtoMappers.Order.Map(updated)));

    private static Fin<Order> Apply(Order order, UpdateOrderCommand request)
    {
        order.ChangeDetails(
            OrderNumber.From(request.Number),
            EffectiveDate.From(request.EffectiveDate));

        if (request.Status is null)
        {
            return order;
        }

        var target = DtoMappers.Status.Map(request.Status.Value);
        if (target == order.Status)
        {
            return order;
        }

        return target switch
        {
            OrderStatus.InProgress => order.Start(),
            OrderStatus.Completed => order.Complete(),
            OrderStatus.Cancelled => order.Cancel(),
            OrderStatus.Rejected => order.Reject(),
            _ => Fin<Order>.Fail(ValidationErrors.ValidationError($"Cannot change order status from {order.Status} to {target}."))
        };
    }
}
