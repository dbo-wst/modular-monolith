using LanguageExt;
using Acme.Core.BuildingBlocks.Domain;

using Acme.Core.Ordering.Domain.Orders;
using Acme.Core.Ordering.Contracts.Commands;
using Acme.Core.Ordering.Contracts.Results;
using Acme.Core.BuildingBlocks.Application.Communication;
using Acme.Core.Ordering.Domain.ValueObjects;
using Acme.Core.BuildingBlocks.Domain.Errors;
using Acme.Core.BuildingBlocks.Domain.Functional;

namespace Acme.Core.Ordering.Application.Orders.CreateOrder;

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
