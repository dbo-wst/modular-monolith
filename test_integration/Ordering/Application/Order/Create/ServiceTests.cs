using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Integration.Dto;
using Template.Core.Ordering.Persistence;
using Template.Core.Ordering.TestCompanion;
using CreateOrderRequest = Template.Core.Ordering.Application.Order.Create;
using GetOrderRequest = Template.Core.Ordering.Application.Order.Get;


namespace Template.Core.Ordering.Application.IntegrationTest.Order.Create;

[Collection(IntegrationTestsFixture.CollectionName)]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly CreateOrderRequest.Service _createService = new(new Store(fixture.GetRequiredService<IOrderingDbContextFactory>()));
    private readonly GetOrderRequest.Service _getService = new(new Store(fixture.GetRequiredService<IOrderingDbContextFactory>()));

    [Fact]
    public async Task Test_Create_Get_Success()
    {
        var createRequest = new Integration.Requests.CreateOrderRequest(
            "ORD-1", new DateOnly(2026, 7, 10));

        var maybeCreated = await _createService.HandleAsync(createRequest);

        Assert.True(maybeCreated.IsSucc);
        var expected = maybeCreated.ToOption().ValueUnsafe();
        Assert.NotNull(expected);

        // A create always starts in the New status.
        Assert.Equal(EStatus.New, expected.Order.Status);
        Assert.Equal(createRequest.Number, expected.Order.OrderNumber);
        Assert.Equal(createRequest.EffectiveDate, expected.Order.EffectiveDate);

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
            null, null);

        var maybeCreated = await _createService.HandleAsync(createRequest);

        Assert.True(maybeCreated.IsSucc);
        var created = maybeCreated.ToOption().ValueUnsafe();
        Assert.NotNull(created);

        Assert.Null(created.Order.OrderNumber);
        Assert.Null(created.Order.EffectiveDate);
        Assert.Equal(EStatus.New, created.Order.Status);

        var maybeOrder = await _getService.HandleAsync(new Integration.Requests.GetOrderRequest(created.Order.OrderId));
        Assert.True(maybeOrder.IsSucc);
        Assert.Equal(created.Order, maybeOrder.ToOption().ValueUnsafe()!.Order);
    }
}
