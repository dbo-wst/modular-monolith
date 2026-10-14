using Template.Core.Ordering.Domain.Order;
using Template.Core.Ordering.Persistence.Order;
using Template.Core.Ordering.Persistence.Order.Mappers;
using Template.Core.Shared.Kernel.Domain;

namespace Template.Core.Ordering.Persistence.IntegrationTest.Order;

public class OrderMapperTests
{
    private static Domain.Order.Order NewOrder(EStatus? status) => new(
        OrderId.From(Guid.NewGuid()), OrderNumber.From("ORD-1"), EffectiveDate.From(new DateOnly(2026, 7, 10)), Status.From(status));

    [Theory]
    [InlineData(EStatus.New, EStatusDbo.New)]
    [InlineData(EStatus.InProgress, EStatusDbo.InProgress)]
    [InlineData(EStatus.Completed, EStatusDbo.Completed)]
    [InlineData(EStatus.Cancelled, EStatusDbo.Cancelled)]
    [InlineData(EStatus.Rejected, EStatusDbo.Rejected)]
    public void Map_ConvertsStatusToDboAndBack(EStatus status, EStatusDbo expectedDbo)
    {
        var mapper = new OrderMapper();
        var order = NewOrder(status);

        var dbo = mapper.Map(order);
        Assert.Equal(expectedDbo, dbo.Status);

        var roundTripped = mapper.Map(dbo);
        Assert.Equal(status, roundTripped.Status?.Value);
    }

    [Fact]
    public void Map_PreservesAllFieldsThroughRoundTrip()
    {
        var mapper = new OrderMapper();
        var order = NewOrder(EStatus.Completed);

        var roundTripped = mapper.Map(mapper.Map(order));

        Assert.Equal(order, roundTripped);
    }

    [Fact]
    public void Map_ConvertsNullStatusToNullDboAndBack()
    {
        var mapper = new OrderMapper();
        var order = NewOrder(null);

        var dbo = mapper.Map(order);
        Assert.Null(dbo.Status);

        var roundTripped = mapper.Map(dbo);
        Assert.Null(roundTripped.Status);
    }
}
