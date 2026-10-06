using Wst.Trios.Core.BrokerSide.Domain.Order;
using Wst.Trios.Core.BrokerSide.Persistence.Order;
using Wst.Trios.Core.BrokerSide.Persistence.Order.Mappers;
using Wst.Trios.Core.Shared.Kernel.Domain;

namespace Wst.Trios.Core.BrokerSide.Persistence.IntegrationTest.Order;

public class OrderMapperTests
{
    private static Domain.Order.Order NewOrder(EStatus? status, ESide? side = ESide.Buy) => new(
        OrderId.From(Guid.NewGuid()), BrokerId.From(Guid.NewGuid()), InstrumentId.From(Guid.NewGuid()), VenueId.From(Guid.NewGuid()), OrderNumber.From("ORD-1"), EffectiveDate.From(new DateOnly(2026, 7, 10)), Quantity.From(100.5m), Price.From(25.75m), Side.From(side), Status.From(status));

    [Theory]
    [InlineData(EStatus.New, EStatusDbo.New)]
    [InlineData(EStatus.PartiallyFilled, EStatusDbo.PartiallyFilled)]
    [InlineData(EStatus.Filled, EStatusDbo.Filled)]
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

    [Theory]
    [InlineData(ESide.Buy, ESideDbo.Buy)]
    [InlineData(ESide.Sell, ESideDbo.Sell)]
    public void Map_ConvertsSideToDboAndBack(ESide side, ESideDbo expectedDbo)
    {
        var mapper = new OrderMapper();
        var order = NewOrder(EStatus.New, side);

        var dbo = mapper.Map(order);
        Assert.Equal(expectedDbo, dbo.Side);

        var roundTripped = mapper.Map(dbo);
        Assert.Equal(side, roundTripped.Side?.Value);
    }

    [Fact]
    public void Map_PreservesAllFieldsThroughRoundTrip()
    {
        var mapper = new OrderMapper();
        var order = NewOrder(EStatus.Filled);

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

    [Fact]
    public void Map_ConvertsNullSideToNullDboAndBack()
    {
        var mapper = new OrderMapper();
        var order = NewOrder(EStatus.New, null);

        var dbo = mapper.Map(order);
        Assert.Null(dbo.Side);

        var roundTripped = mapper.Map(dbo);
        Assert.Null(roundTripped.Side);
    }
}
