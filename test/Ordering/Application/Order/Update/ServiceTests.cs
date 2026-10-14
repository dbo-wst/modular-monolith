using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Domain.Order;
using Template.Core.Ordering.Persistence;
using Template.Core.Ordering.TestCompanion;
using CreateOrderRequest = Template.Core.Ordering.Application.Order.Create;
using EStatus = Template.Core.Ordering.Integration.Dto.EStatus;
using GetOrderRequest = Template.Core.Ordering.Application.Order.Get;
using UpdateOrderRequest = Template.Core.Ordering.Application.Order.Update;

namespace Template.Core.Ordering.Application.Test.Order.Update;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly CreateOrderRequest.Service _createService = new(new Store(fixture.ContextFactory));
    private readonly UpdateOrderRequest.Service _updateService = new(new Store(fixture.ContextFactory));
    private readonly GetOrderRequest.Service _getService = new(new Store(fixture.ContextFactory));

    private const string BaselineNumber = "ORD-1";
    private static readonly DateOnly BaselineEffectiveDate = new(2026, 7, 10);
    private const EStatus BaselineStatus = EStatus.New;

    // --- Success path ---------------------------------------------------------

    [Fact]
    public async Task Test_Update_Get_Success()
    {
        var order = await CreateBaselineAsync();

        var newEffectiveDate = new DateOnly(2026, 7, 11);
        var maybeUpdated = await _updateService.HandleAsync(new Integration.Requests.UpdateOrderRequest(
            order.OrderId, "ORD-2", newEffectiveDate, EStatus.Completed));

        Assert.True(maybeUpdated.IsSucc);
        var updated = maybeUpdated.ToOption().ValueUnsafe();
        Assert.NotNull(updated);
        Assert.Equal(order.OrderId, updated.Order.OrderId);
        Assert.Equal("ORD-2", updated.Order.OrderNumber);
        Assert.Equal(newEffectiveDate, updated.Order.EffectiveDate);
        Assert.Equal(EStatus.Completed, updated.Order.Status);

        // Persisted (round-trips through a fresh Get)
        var maybeOrder = await _getService.HandleAsync(new Integration.Requests.GetOrderRequest(order.OrderId));
        Assert.True(maybeOrder.IsSucc);
        var actual = maybeOrder.ToOption().ValueUnsafe();
        Assert.NotNull(actual);
        Assert.Equal(updated.Order, actual.Order);
    }

    // --- Error case -----------------------------------------------------------

    [Fact]
    public async Task Test_Update_MissingEntity_ReturnsFail()
    {
        var missingId = OrderId.From(Guid.NewGuid());

        var actual = await _updateService.HandleAsync(new Integration.Requests.UpdateOrderRequest(
            missingId.Value, null, null, EStatus.Completed));

        Assert.True(actual.IsFail);
        var message = actual.Match(Succ: _ => string.Empty, Fail: error => error.Message);
        Assert.Equal(PersistenceErrors.NotFound(missingId).Message, message);
    }

    // --- Single-field updates -------------------------------------------------
    // "null = not updated": every request field except OrderId is nullable, and a
    // null field is left untouched. Each test below sets exactly one field and
    // asserts nothing else moved from its baseline.

    [Fact]
    public async Task Test_Update_OnlyNumber_ChangesNumber_PreservesRest()
    {
        var order = await CreateBaselineAsync();

        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            order.OrderId, "ORD-2", null, null));

        Assert.Equal("ORD-2", updated.OrderNumber);
        AssertUnchangedExcept(updated, order.OrderId, numberShouldBe: "ORD-2");
    }

    [Fact]
    public async Task Test_Update_OnlyEffectiveDate_ChangesEffectiveDate_PreservesRest()
    {
        var order = await CreateBaselineAsync();
        var newEffectiveDate = new DateOnly(2026, 7, 11);

        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            order.OrderId, null, newEffectiveDate, null));

        Assert.Equal(newEffectiveDate, updated.EffectiveDate);
        AssertUnchangedExcept(updated, order.OrderId, effectiveDateShouldBe: newEffectiveDate);
    }

    [Fact]
    public async Task Test_Update_OnlyStatus_ChangesStatus_PreservesRest()
    {
        var order = await CreateBaselineAsync();

        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            order.OrderId, null, null, EStatus.Completed));

        Assert.Equal(EStatus.Completed, updated.Status);
        AssertUnchangedExcept(updated, order.OrderId, statusShouldBe: EStatus.Completed);
    }

    [Fact]
    public async Task Test_Update_AllNullExceptId_IsNoOp()
    {
        var order = await CreateBaselineAsync();

        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            order.OrderId, null, null, null));

        AssertUnchangedExcept(updated, order.OrderId);
        Assert.Equal(order, updated);
    }

    // --- Helpers --------------------------------------------------------------

    private async Task<Integration.Dto.Order> CreateBaselineAsync()
    {
        var maybeCreated = await _createService.HandleAsync(
            new Integration.Requests.CreateOrderRequest(BaselineNumber, BaselineEffectiveDate));

        Assert.True(maybeCreated.IsSucc);
        var created = maybeCreated.ToOption().ValueUnsafe();
        Assert.NotNull(created);
        return created.Order;
    }

    private async Task<Integration.Dto.Order> UpdateAsync(Integration.Requests.UpdateOrderRequest request)
    {
        var maybeUpdated = await _updateService.HandleAsync(request);
        Assert.True(maybeUpdated.IsSucc);
        var updated = maybeUpdated.ToOption().ValueUnsafe();
        Assert.NotNull(updated);
        return updated.Order;
    }

    // Asserts every field except the one(s) explicitly overridden still holds baseline.
    private static void AssertUnchangedExcept(
        Integration.Dto.Order updated,
        Guid orderId,
        string? numberShouldBe = null,
        DateOnly? effectiveDateShouldBe = null,
        EStatus? statusShouldBe = BaselineStatus)
    {
        Assert.Equal(orderId, updated.OrderId);
        Assert.Equal(numberShouldBe ?? BaselineNumber, updated.OrderNumber);
        Assert.Equal(effectiveDateShouldBe ?? BaselineEffectiveDate, updated.EffectiveDate);
        Assert.Equal(statusShouldBe, updated.Status);
    }
}
