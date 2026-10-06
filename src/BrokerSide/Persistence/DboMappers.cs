using Wst.Trios.Core.BrokerSide.Persistence.Order.Mappers;
using TradeMapper = Wst.Trios.Core.BrokerSide.Persistence.Trade.Mappers.TradeMapper;

namespace Wst.Trios.Core.BrokerSide.Persistence;

public static class DboMappers
{
    public static readonly OrderMapper Order = default;
    public static readonly TradeMapper Trade = default;
}
