using Wst.Trios.Core.Shared.Kernel.Domain;

namespace Wst.Trios.Core.BrokerSide.Domain.Order.Events;

public sealed record OrderUpdated(
    OrderId OrderId,
    BrokerId? BrokerId = null,
    InstrumentId? InstrumentId = null,
    VenueId? VenueId = null,
    OrderNumber? Number = null,
    EffectiveDate? EffectiveDate = null,
    Quantity? Quantity = null,
    Price? Price = null,
    Side? Side = null,
    Status? Status = null) : DomainEvent;
