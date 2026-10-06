using Wst.Trios.Core.Shared.Kernel.Domain;

namespace Wst.Trios.Core.BrokerSide.Domain.Order;

public record Order(
    OrderId OrderId,
    BrokerId? BrokerId,
    InstrumentId? InstrumentId,
    VenueId? VenueId,
    OrderNumber? OrderNumber,
    EffectiveDate? EffectiveDate,
    Quantity? Quantity,
    Price? Price,
    Side? Side,
    Status? Status);
