using Wst.Trios.Core.Shared.Kernel.Domain;

namespace Wst.Trios.Core.BrokerSide.Domain.Order.Events;

public sealed record OrderDeleted(OrderId OrderId) : DomainEvent;
