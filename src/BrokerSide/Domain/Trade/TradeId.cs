using LanguageExt;

namespace Wst.Trios.Core.BrokerSide.Domain.Trade;

public record TradeId
{
    private TradeId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static Fin<TradeId> MaybeCreate(Guid tradeId) => Fin<TradeId>.Succ(new TradeId(tradeId));
    public static TradeId From(Guid tradeId) => new(tradeId);

    public static TradeId? From(Guid? tradeId) => tradeId.HasValue ? new TradeId(tradeId.Value) : null;
}
