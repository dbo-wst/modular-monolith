using LanguageExt.UnsafeValueAccess;
using Wst.Trios.Core.BrokerSide.Integration.Dto;
using Wst.Trios.Core.BrokerSide.Persistence;
using Wst.Trios.Core.BrokerSide.TestCompanion;
using CreateOrderRequest = Wst.Trios.Core.BrokerSide.Application.Order.Create;
using GetOrderRequest = Wst.Trios.Core.BrokerSide.Application.Order.Get;


namespace Wst.Trios.Core.BrokerSide.Application.IntegrationTest.Order.Create;

[Collection(IntegrationTestsFixture.CollectionName)]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly CreateOrderRequest.Service _createService = new(new Store(fixture.GetRequiredService<IBrokerSideDbContextFactory>()));
    private readonly GetOrderRequest.Service _getService = new(new Store(fixture.GetRequiredService<IBrokerSideDbContextFactory>()));

    [Fact]
    public async Task Test_Create_Get_Success()
    {
        var createRequest = new Integration.Requests.CreateOrderRequest(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "ORD-1", new DateOnly(2026, 7, 10), 100.5m, 25.75m, ESide.Buy);

        var maybeCreated = await _createService.HandleAsync(createRequest);

        Assert.True(maybeCreated.IsSucc);
        var expected = maybeCreated.ToOption().ValueUnsafe();
        Assert.NotNull(expected);

        // A create always starts in the New status.
        Assert.Equal(EStatus.New, expected.Order.Status);
        Assert.Equal(createRequest.BrokerId, expected.Order.BrokerId);
        Assert.Equal(createRequest.InstrumentId, expected.Order.InstrumentId);
        Assert.Equal(createRequest.VenueId, expected.Order.VenueId);
        Assert.Equal(createRequest.Number, expected.Order.OrderNumber);
        Assert.Equal(createRequest.EffectiveDate, expected.Order.EffectiveDate);
        Assert.Equal(createRequest.Quantity, expected.Order.Quantity);
        Assert.Equal(createRequest.Price, expected.Order.Price);
        Assert.Equal(createRequest.Side, expected.Order.Side);

        var getRequest = new Integration.Requests.GetOrderRequest(expected.Order.OrderId);
        var maybeOrder = await _getService.HandleAsync(getRequest);

        Assert.True(maybeOrder.IsSucc);
        var actual = maybeOrder.ToOption().ValueUnsafe();
        Assert.NotNull(actual);

        Assert.Equal(expected.Order, actual.Order);
    }

    [Fact]
    public async Task Test_Create_WithNullOptionalFields_Success()
    {
        var createRequest = new Integration.Requests.CreateOrderRequest(
            null, null, null, null, null, null, null, null);

        var maybeCreated = await _createService.HandleAsync(createRequest);

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

        var maybeOrder = await _getService.HandleAsync(new Integration.Requests.GetOrderRequest(created.Order.OrderId));
        Assert.True(maybeOrder.IsSucc);
        Assert.Equal(created.Order, maybeOrder.ToOption().ValueUnsafe()!.Order);
    }
}
