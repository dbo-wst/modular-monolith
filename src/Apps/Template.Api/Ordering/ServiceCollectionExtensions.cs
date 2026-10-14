using Template.Api.Ordering.Order;
using Template.Api.Ordering.OrderItem;

namespace Template.Api.Ordering;

public static class ServiceCollectionExtensions
{

    public static void MapOrderingEndpoints(this WebApplication app)
    {
        app.MapOrderEndpoints();
        app.MapOrderItemEndpoints();
    }
}
