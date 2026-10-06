using Wst.Trios.Core.BrokerSide.Integration.Dto;
using Wst.Trios.Core.BrokerSide.Integration.Responses;
using Wst.Trios.Core.Shared.Kernel.Communication;

namespace Wst.Trios.Core.BrokerSide.Integration.Requests;

public sealed record UpdateOrderRequest(
    Guid OrderId,
    Guid? BrokerId,
    Guid? InstrumentId,
    Guid? VenueId,
    string? Number,
    DateOnly? EffectiveDate,
    decimal? Quantity,
    decimal? Price,
    ESide? Side,
    EStatus? Status) : IRequest<UpdateOrderResponse>;
