using Wst.Trios.Core.Shared.Kernel.Domain;

namespace Wst.Trios.Core.BrokerSide.Domain.Trade.Events;

public sealed record TradeUpdated(
    TradeId TradeId,
    OrderId? OrderId = null,
    EffectiveDate? EffectiveDate = null,
    Quantity? Qty = null,
    Price? Price = null,
    Amount? Amount = null) : DomainEvent;
