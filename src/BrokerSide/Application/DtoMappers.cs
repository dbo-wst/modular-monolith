using Wst.Trios.Core.BrokerSide.Application.Order.Mappers;
using TradeMapper = Wst.Trios.Core.BrokerSide.Application.Trade.Mappers.TradeMapper;

namespace Wst.Trios.Core.BrokerSide.Application;

public static class DtoMappers
{
    public static readonly OrderMapper Order = default;
    public static readonly SideMapper Side = default;
    public static readonly StatusMapper Status = default;
    public static readonly TradeMapper Trade = default;
}
