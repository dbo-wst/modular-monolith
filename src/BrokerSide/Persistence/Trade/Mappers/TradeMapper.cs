using Wst.Trios.Core.Shared.Kernel;
using Wst.Trios.Core.Shared.Kernel.Domain;
using TradeId = Wst.Trios.Core.BrokerSide.Domain.Trade.TradeId;
using OrderId = Wst.Trios.Core.BrokerSide.Domain.Trade.OrderId;

namespace Wst.Trios.Core.BrokerSide.Persistence.Trade.Mappers;

public struct TradeMapper : IMap<Domain.Trade.Trade, TradeDbo>
{
    public Domain.Trade.Trade Map(TradeDbo right)
    {
        return new Domain.Trade.Trade(
            TradeId.From(right.TradeId),
            OrderId.From(right.OrderId),
            EffectiveDate.From(right.EffectiveDate),
            Quantity.From(right.Quantity),
            Price.From(right.Price),
            Amount.From(right.Amount));
    }

    public TradeDbo Map(Domain.Trade.Trade left)
    {
        return new TradeDbo
        {
            TradeId = left.TradeId.Value,
            OrderId = left.OrderId?.Value,
            EffectiveDate = left.EffectiveDate?.Value,
            Quantity = left.Quantity?.Value,
            Price = left.Price?.Value,
            Amount = left.Amount?.Value
        };
    }
}
