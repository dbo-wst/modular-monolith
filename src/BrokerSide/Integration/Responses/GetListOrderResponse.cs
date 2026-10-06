using System.Collections.Immutable;
using Wst.Trios.Core.BrokerSide.Integration.Dto;

namespace Wst.Trios.Core.BrokerSide.Integration.Responses;

public sealed record GetListOrderResponse(ImmutableList<Order> Orders);
