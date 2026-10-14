using Template.Core.Ordering.Persistence.OrderItem;

namespace Template.Core.Ordering.Persistence.IntegrationTest.OrderItem;

public class WriteOnlyOperationsTests
{
    [Fact]
    public void OrderItemDboRoundTripsThroughTheMapper()
    {
        var dbo = new OrderItemDbo
        {
            OrderItemId = Guid.NewGuid(),
            OrderId = Guid.NewGuid(),
            Quantity = 100.5m,
            Price = 25.75m,
        };

        var domain = DboMappers.OrderItem.Map(dbo);

        Assert.Equal(dbo.OrderId, domain.OrderId?.Value);
        Assert.Equal(dbo.Quantity, domain.Quantity?.Value);
    }
}

