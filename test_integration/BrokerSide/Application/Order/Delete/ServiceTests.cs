using LanguageExt.UnsafeValueAccess;
using Wst.Trios.Core.BrokerSide.Persistence;
using Wst.Trios.Core.BrokerSide.TestCompanion;
using CreateOrderRequest = Wst.Trios.Core.BrokerSide.Application.Order.Create;
using DeleteOrderRequest = Wst.Trios.Core.BrokerSide.Application.Order.Delete;
using ESide = Wst.Trios.Core.BrokerSide.Integration.Dto.ESide;
using GetOrderRequest = Wst.Trios.Core.BrokerSide.Application.Order.Get;
using OrderId = Wst.Trios.Core.BrokerSide.Domain.Order.OrderId;

namespace Wst.Trios.Core.BrokerSide.Application.IntegrationTest.Order.Delete;

[Collection(IntegrationTestsFixture.CollectionName)]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly CreateOrderRequest.Service _createService = new(new Store(fixture.GetRequiredService<IBrokerSideDbContextFactory>()));
    private readonly DeleteOrderRequest.Service _deleteService = new(new Store(fixture.GetRequiredService<IBrokerSideDbContextFactory>()));
    private readonly GetOrderRequest.Service _getService = new(new Store(fixture.GetRequiredService<IBrokerSideDbContextFactory>()));

    [Fact]
    public async Task Test_Delete_Success()
    {
        var maybeCreated = await _createService.HandleAsync(new Integration.Requests.CreateOrderRequest(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "ORD-1", new DateOnly(2026, 7, 10), 100.5m, 25.75m, ESide.Buy));
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
