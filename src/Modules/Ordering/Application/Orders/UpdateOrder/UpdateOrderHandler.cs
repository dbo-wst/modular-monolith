using LanguageExt;
using Acme.Core.BuildingBlocks.Domain;
using Acme.Core.Ordering.Domain.Orders;
using Acme.Core.Ordering.Contracts.Commands;
using Acme.Core.Ordering.Contracts.Results;
using Acme.Core.BuildingBlocks.Application.Communication;
using Acme.Core.Ordering.Domain.ValueObjects;
using Acme.Core.BuildingBlocks.Domain.Errors;
using Acme.Core.BuildingBlocks.Domain.Functional;

namespace Acme.Core.Ordering.Application.Orders.UpdateOrder;

internal sealed class UpdateOrderHandler(Domain.Orders.IOrderRepository orders, IUnitOfWork unitOfWork): ICommandHandler<UpdateOrderCommand, UpdateOrderResult>
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
            _ => Fin<Order>.Fail(ValidationError.New($"Cannot change order status from {order.Status} to {target}."))
        };
    }
}
