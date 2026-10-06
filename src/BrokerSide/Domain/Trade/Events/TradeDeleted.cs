using Wst.Trios.Core.Shared.Kernel.Domain;

namespace Wst.Trios.Core.BrokerSide.Domain.Trade.Events;

public sealed record TradeDeleted(TradeId TradeId) : DomainEvent;
