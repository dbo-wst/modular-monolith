using Template.Core.Shared.Kernel;

namespace Template.Core.Ordering.Application.Order.Mappers;

public readonly struct OrderMapper : IOnewayMap<Domain.Order.Order, Integration.Dto.Order>
{
    public Integration.Dto.Order Map(Domain.Order.Order left) => new(
        left.OrderId.Value,
        left.OrderNumber?.Value,
        left.EffectiveDate?.Value,
        left.Status != null ? DtoMappers.Status.Map(left.Status.Value) : null);
}
