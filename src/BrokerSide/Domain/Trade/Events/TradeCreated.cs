using Wst.Trios.Core.Shared.Kernel.Domain;

namespace Wst.Trios.Core.BrokerSide.Domain.Trade.Events;

public sealed record TradeCreated(
    TradeId TradeId,
    OrderId? OrderId,
    EffectiveDate? EffectiveDate,
    Quantity? Qty,
    Price? Price,
    Amount? Amount) : DomainEvent;
