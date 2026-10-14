using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Integration.Dto;
using Template.Core.Ordering.Persistence;
using Template.Core.Ordering.TestCompanion;
using CreateOrderRequest = Template.Core.Ordering.Application.Order.Create;
using GetOrderRequest = Template.Core.Ordering.Application.Order.Get;

namespace Template.Core.Ordering.Application.Test.Order.Create;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly CreateOrderRequest.Service _createService = new(new Store(fixture.ContextFactory));
    private readonly GetOrderRequest.Service _getService = new(new Store(fixture.ContextFactory));

    [Fact]
    public async Task Test_create_Get_Success()
    {
        var createRequest = new Integration.Requests.CreateOrderRequest("ORD-1", new DateOnly(2026, 7, 10));

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
        var maybeCreated = await _createService.HandleAsync(
            new Integration.Requests.CreateOrderRequest("ORD-1", new DateOnly(2026, 7, 10)));

        Assert.True(maybeCreated.IsSucc);
        var created = maybeCreated.ToOption().ValueUnsafe();
        Assert.NotNull(created);
        Assert.NotEqual(Guid.Empty, created.Order.OrderId);
        Assert.Equal(EStatus.New, created.Order.Status);
    }

    [Fact]
    public async Task Test_Create_WithNullableFields_Succeeds()
    {
        var maybeCreated = await _createService.HandleAsync(
            new Integration.Requests.CreateOrderRequest(null, null));

        Assert.True(maybeCreated.IsSucc);
        var created = maybeCreated.ToOption().ValueUnsafe();
        Assert.NotNull(created);
        Assert.Null(created.Order.OrderNumber);
        Assert.Null(created.Order.EffectiveDate);
        Assert.Equal(EStatus.New, created.Order.Status);
    }
}
