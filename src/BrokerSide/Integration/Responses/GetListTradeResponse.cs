using System.Collections.Immutable;
using Wst.Trios.Core.BrokerSide.Integration.Dto;

namespace Wst.Trios.Core.BrokerSide.Integration.Responses;

public sealed record GetListTradeResponse(ImmutableList<Trade> Trades);
