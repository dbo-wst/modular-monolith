using Acme.Core.Ordering.Persistence.OrderItems;

namespace Acme.Core.Ordering.Persistence.IntegrationTest.OrderItems;

public class OrderItemMapperTests
{
    private static OrderItemDbo NewDbo() => new()
    {
        OrderItemId = Guid.NewGuid(),
        OrderId = Guid.NewGuid(),
        Quantity = 100.5m,
        Price = 25.75m,
    };

    [Fact]
    public void Map_Dbo_To_Domain_AndBack_RoundTrips()
    {
        var dbo = NewDbo();

        var domain = DboMappers.OrderItem.Map(dbo);
        var roundTripped = DboMappers.OrderItem.Map(domain);

        Assert.Equal(dbo.OrderItemId, roundTripped.OrderItemId);
        Assert.Equal(dbo.OrderId, roundTripped.OrderId);
        Assert.Equal(dbo.Quantity, roundTripped.Quantity);
        Assert.Equal(dbo.Price, roundTripped.Price);
    }
}

