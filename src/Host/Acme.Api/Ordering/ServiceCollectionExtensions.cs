using Acme.Api.Ordering.Order;
using Acme.Api.Ordering.OrderItem;

namespace Acme.Api.Ordering;

public static class ServiceCollectionExtensions
{

    public static void MapOrderingEndpoints(this WebApplication app)
    {
        app.MapOrderEndpoints();
        app.MapOrderItemEndpoints();
    }
}
