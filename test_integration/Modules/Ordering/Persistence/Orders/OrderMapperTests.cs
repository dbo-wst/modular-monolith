using Acme.Core.Ordering.Domain.Orders;
using Acme.Core.Ordering.Persistence.Orders;
using Acme.Core.Ordering.Persistence.Orders.Mappers;
using Acme.Core.Ordering.Domain.ValueObjects;

namespace Acme.Core.Ordering.Persistence.IntegrationTest.Orders;

public class OrderMapperTests
{
    private static Domain.Orders.Order NewOrder(OrderStatus? status) => Domain.Orders.Order.Rehydrate(
        OrderId.From(Guid.NewGuid()), OrderNumber.From("ORD-1"), EffectiveDate.From(new DateOnly(2026, 7, 10)), status);

    [Theory]
    [InlineData(OrderStatus.New, EStatusDbo.New)]
    [InlineData(OrderStatus.InProgress, EStatusDbo.InProgress)]
    [InlineData(OrderStatus.Completed, EStatusDbo.Completed)]
    [InlineData(OrderStatus.Cancelled, EStatusDbo.Cancelled)]
    [InlineData(OrderStatus.Rejected, EStatusDbo.Rejected)]
    public void Map_ConvertsStatusToDboAndBack(OrderStatus status, EStatusDbo expectedDbo)
    {
        var mapper = new OrderMapper();
        var order = NewOrder(status);

        var dbo = mapper.Map(order);
        Assert.Equal(expectedDbo, dbo.Status);

        var roundTripped = mapper.Map(dbo);
        Assert.Equal(status, roundTripped.Status);
    }

    private static void AssertSame(Domain.Orders.Order expected, Domain.Orders.Order actual)
    {
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.OrderNumber, actual.OrderNumber);
        Assert.Equal(expected.EffectiveDate, actual.EffectiveDate);
        Assert.Equal(expected.Status, actual.Status);
    }

    [Fact]
    public void Map_PreservesAllFieldsThroughRoundTrip()
    {
        var mapper = new OrderMapper();
        var order = NewOrder(OrderStatus.Completed);

        var roundTripped = mapper.Map(mapper.Map(order));

        AssertSame(order, roundTripped);
    }

    [Fact]
    public void Map_MissingStatusInDbo_RehydratesAsNew_AndIsPersistedExplicitly()
    {
        var mapper = new OrderMapper();
        var dbo = mapper.Map(NewOrder(OrderStatus.New));
        dbo.Status = null;

        var rehydrated = mapper.Map(dbo);
        Assert.Equal(OrderStatus.New, rehydrated.Status);

        Assert.Equal(EStatusDbo.New, mapper.Map(rehydrated).Status);
    }
}
