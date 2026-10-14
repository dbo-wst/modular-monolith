using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Persistence;
using Template.Core.Ordering.TestCompanion;
using CreateOrderRequest = Template.Core.Ordering.Application.Order.Create;
using DeleteOrderRequest = Template.Core.Ordering.Application.Order.Delete;
using GetOrderRequest = Template.Core.Ordering.Application.Order.Get;
using OrderId = Template.Core.Ordering.Domain.Order.OrderId;

namespace Template.Core.Ordering.Application.IntegrationTest.Order.Delete;

[Collection(IntegrationTestsFixture.CollectionName)]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly CreateOrderRequest.Service _createService = new(new Store(fixture.GetRequiredService<IOrderingDbContextFactory>()));
    private readonly DeleteOrderRequest.Service _deleteService = new(new Store(fixture.GetRequiredService<IOrderingDbContextFactory>()));
    private readonly GetOrderRequest.Service _getService = new(new Store(fixture.GetRequiredService<IOrderingDbContextFactory>()));

    [Fact]
    public async Task Test_Delete_Success()
    {
        var maybeCreated = await _createService.HandleAsync(new Integration.Requests.CreateOrderRequest(
            "ORD-1", new DateOnly(2026, 7, 10)));
        Assert.True(maybeCreated.IsSucc);
        var created = maybeCreated.ToOption().ValueUnsafe();
        Assert.NotNull(created);

        var actual = await _deleteService.HandleAsync(new Integration.Requests.DeleteOrderRequest(created.Order.OrderId));

        Assert.True(actual.IsSucc);

        var maybeOrder = await _getService.HandleAsync(new Integration.Requests.GetOrderRequest(created.Order.OrderId));
        Assert.True(maybeOrder.IsFail);
    }

    [Fact]
    public async Task Test_Delete_MissingEntity_ReturnsFail()
    {
        var missingId = OrderId.From(Guid.NewGuid());

        var actual = await _deleteService.HandleAsync(new Integration.Requests.DeleteOrderRequest(missingId.Value));

        Assert.True(actual.IsFail);
        var message = actual.Match(Succ: _ => string.Empty, Fail: error => error.Message);
        Assert.Equal(PersistenceErrors.NotFound(missingId).Message, message);
    }
}
