using Wst.Trios.Core.Shared.Kernel;

namespace Wst.Trios.Core.BrokerSide.Application.Order.Mappers;

public readonly struct OrderMapper : IOnewayMap<Domain.Order.Order, Integration.Dto.Order>
{
    public Integration.Dto.Order Map(Domain.Order.Order left) => new(
        left.OrderId.Value,
        left.BrokerId?.Value,
        left.InstrumentId?.Value,
        left.VenueId?.Value,
        left.OrderNumber?.Value,
        left.EffectiveDate?.Value,
        left.Quantity?.Value,
        left.Price?.Value,
        left.Side != null ? DtoMappers.Side.Map(left.Side.Value) : null,
        left.Status != null ? DtoMappers.Status.Map(left.Status.Value) : null);
}
