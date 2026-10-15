using Acme.Core.BuildingBlocks.Domain;

namespace Acme.Core.Ordering.Application.Orders.Mappers;

public readonly struct OrderMapper : IOnewayMap<Domain.Orders.Order, Contracts.Dto.Order>
{
    public Contracts.Dto.Order Map(Domain.Orders.Order left) => new(
        left.Id.Value,
        left.OrderNumber.Value,
        left.EffectiveDate.Value,
        DtoMappers.Status.Map(left.Status));
}
