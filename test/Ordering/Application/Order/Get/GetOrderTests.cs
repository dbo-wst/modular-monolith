using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Integration.Dto;
using Template.Core.Ordering.Persistence;
using Template.Core.Ordering.TestCompanion;
using CreateOrderRequest = Template.Core.Ordering.Application.Order.Create;
using GetOrderRequest = Template.Core.Ordering.Application.Order.Get;
using OrderId = Template.Core.Ordering.Domain.Order.OrderId;

namespace Template.Core.Ordering.Application.Test.Order.Get;

public class GetOrderTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly CreateOrderRequest.Service _createService = new(new Store(fixture.ContextFactory));
    private readonly GetOrderRequest.Service _getService = new(new Store(fixture.ContextFactory));

    [Fact]
    public async Task Test_Get_Success()
    {
        var createRequest = new Integration.Requests.CreateOrderRequest(
            "ORD-1", new DateOnly(2026, 7, 10));

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
    public async Task Test_Get_MissingEntity_ReturnsFail()
    {
        var missingId = OrderId.From(Guid.NewGuid());

        var actual = await _getService.HandleAsync(new Integration.Requests.GetOrderRequest(missingId.Value));

        Assert.True(actual.IsFail);
        var message = actual.Match(Succ: _ => string.Empty, Fail: error => error.Message);
        Assert.Equal(PersistenceErrors.NotFound(missingId).Message, message);
    }
}
