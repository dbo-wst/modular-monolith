using Wst.Trios.Api.BrokerSide.Order;
using Wst.Trios.Api.BrokerSide.Trade;

namespace Wst.Trios.Api.BrokerSide;

public static class ServiceCollectionExtensions
{

    public static void MapBrokerSideEndpoints(this WebApplication app)
    {
        app.MapOrderEndpoints();
        app.MapTradeEndpoints();
    }
}
