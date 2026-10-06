using LanguageExt.UnsafeValueAccess;
using Wst.Trios.Core.BrokerSide.Integration.Dto;
using Wst.Trios.Core.BrokerSide.Persistence;
using Wst.Trios.Core.BrokerSide.TestCompanion;
using CreateOrderRequest = Wst.Trios.Core.BrokerSide.Application.Order.Create;
using GetOrderRequest = Wst.Trios.Core.BrokerSide.Application.Order.Get;

namespace Wst.Trios.Core.BrokerSide.Application.Test.Order.Create;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly CreateOrderRequest.Service _createService = new(new Store(fixture.ContextFactory));
    private readonly GetOrderRequest.Service _getService = new(new Store(fixture.ContextFactory));

    [Fact]
    public async Task Test_create_Get_Success()
    {
        var createRequest = new Integration.Requests.CreateOrderRequest(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "ORD-1", new DateOnly(2026, 7, 10), 100.5m, 25.75m, ESide.Buy);

        var maybeCreated = await _createService.HandleAsync(createRequest);

        Assert.True(maybeCreated.IsSucc);
        var expected = maybeCreated.ToOption().ValueUnsafe();
        Assert.NotNull(expected);

        var getRequest = new Integration.Requests.GetOrderRequest(expected.Order.OrderId);
        var maybeOrder = await _getService.HandleAsync(getRequest);

        Assert.True(maybeOrder.IsSucc);
        var actual = maybeOrder.ToOption().ValueUnsafe();
        Assert.NotNull(actual);

        Assert.Equal(expected.Order, actual.Order);
    }

    [Fact]
    public async Task Test_Create_AssignsServerSideOrderId()
    {
        var maybeCreated = await _createService.HandleAsync(new Integration.Requests.CreateOrderRequest(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "ORD-1", new DateOnly(2026, 7, 10), 100.5m, 25.75m, ESide.Buy));

        Assert.True(maybeCreated.IsSucc);
        var created = maybeCreated.ToOption().ValueUnsafe();
        Assert.NotNull(created);
        Assert.NotEqual(Guid.Empty, created.Order.OrderId);
        Assert.Equal(EStatus.New, created.Order.Status);
    }

    [Fact]
    public async Task Test_Create_WithNullableFields_Succeeds()
    {
        var maybeCreated = await _createService.HandleAsync(new Integration.Requests.CreateOrderRequest(
            null, null, null, null, null, null, null, null));

        Assert.True(maybeCreated.IsSucc);
        var created = maybeCreated.ToOption().ValueUnsafe();
        Assert.NotNull(created);
        Assert.Null(created.Order.BrokerId);
        Assert.Null(created.Order.InstrumentId);
        Assert.Null(created.Order.VenueId);
        Assert.Null(created.Order.OrderNumber);
        Assert.Null(created.Order.EffectiveDate);
        Assert.Null(created.Order.Quantity);
        Assert.Null(created.Order.Price);
        Assert.Null(created.Order.Side);
        Assert.Equal(EStatus.New, created.Order.Status);
    }
}
