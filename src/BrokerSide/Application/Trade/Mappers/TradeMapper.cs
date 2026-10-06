using Wst.Trios.Core.Shared.Kernel;

namespace Wst.Trios.Core.BrokerSide.Application.Trade.Mappers;

public readonly struct TradeMapper : IOnewayMap<Wst.Trios.Core.BrokerSide.Domain.Trade.Trade, Integration.Dto.Trade>
{
    public Integration.Dto.Trade Map(Wst.Trios.Core.BrokerSide.Domain.Trade.Trade left) => new(
        left.TradeId.Value,
        left.OrderId?.Value,
        left.EffectiveDate?.Value,
        left.Quantity?.Value,
        left.Price?.Value,
        left.Amount?.Value);
}

