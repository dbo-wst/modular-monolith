using LanguageExt;
using Template.Core.BuildingBlocks.Domain;

using Template.Core.Ordering.Domain.Orders;
using Template.Core.Ordering.Contracts.Commands;
using Template.Core.Ordering.Contracts.Results;
using Template.Core.BuildingBlocks.Application.Communication;
using Template.Core.Ordering.Domain.ValueObjects;
using Template.Core.BuildingBlocks.Domain.Errors;
using Template.Core.BuildingBlocks.Domain.Functional;

namespace Template.Core.Ordering.Application.Orders.CreateOrder;

internal sealed class CreateOrderHandler(Domain.Orders.IOrderRepository orders, IUnitOfWork unitOfWork) : ICommandHandler<CreateOrderCommand, CreateOrderResult>
{
    public Task<Fin<CreateOrderResult>> HandleAsync(CreateOrderCommand request, CancellationToken ct = default) =>
        Build(request)
            .AsTask()
            .BindAsync(order => orders.AddAsync(order, ct))
            .BindAsync(created => unitOfWork.SaveChangesAsync(ct).MapAsync(_ => created))
            .MapAsync(created => new CreateOrderResult(DtoMappers.Order.Map(created)));

    private static Fin<Order> Build(CreateOrderCommand request)
    {
        var number = OrderNumber.From(request.Number);
        var effectiveDate = EffectiveDate.From(request.EffectiveDate);

        return number is null || effectiveDate is null
            ? Fin<Order>.Fail(ValidationError.New("Order number and effective date are required."))
            : Order.Create(number, effectiveDate);
    }
}
