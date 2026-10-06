using Wst.Trios.Core.Shared.Kernel.Domain;

namespace Wst.Trios.Core.BrokerSide.Domain.Trade;

public record Trade(
    TradeId TradeId,
    OrderId? OrderId,
    EffectiveDate? EffectiveDate,
    Quantity? Quantity,
    Price? Price,
    Amount? Amount);
