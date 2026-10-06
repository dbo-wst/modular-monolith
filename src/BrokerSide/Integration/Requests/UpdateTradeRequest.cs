using Wst.Trios.Core.BrokerSide.Integration.Responses;
using Wst.Trios.Core.Shared.Kernel.Communication;

namespace Wst.Trios.Core.BrokerSide.Integration.Requests;

public sealed record UpdateTradeRequest(
    Guid TradeId,
    Guid? OrderId = null,
    DateOnly? EffectiveDate = null,
    decimal? Quantity = null,
    decimal? Price = null,
    decimal? Amount = null) : IRequest<UpdateTradeResponse>;

