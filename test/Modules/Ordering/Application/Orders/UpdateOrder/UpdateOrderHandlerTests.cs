using Acme.Core.BuildingBlocks.Domain.Errors;
using LanguageExt.UnsafeValueAccess;
using Acme.Core.Ordering.Domain.Orders;
using Acme.Core.Ordering.Persistence;
using Acme.Core.Ordering.TestCompanion;
using CreateOrderHandler = Acme.Core.Ordering.Application.Orders.CreateOrder.CreateOrderHandler;
using EStatus = Acme.Core.Ordering.Contracts.Dto.EStatus;
using GetOrderHandler = Acme.Core.Ordering.Application.Orders.GetOrder.GetOrderHandler;
using UpdateOrderHandler = Acme.Core.Ordering.Application.Orders.UpdateOrder.UpdateOrderHandler;

namespace Acme.Core.Ordering.Application.Test.Orders.UpdateOrder;

public class UpdateOrderHandlerTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly CreateOrderHandler _createService = OrderingPersistence.Create(fixture.ContextFactory).ForOrders((r, u) => new CreateOrderHandler(r, u));
    private readonly UpdateOrderHandler _updateService = OrderingPersistence.Create(fixture.ContextFactory).ForOrders((r, u) => new UpdateOrderHandler(r, u));
    private readonly GetOrderHandler _getService = new(new ReadStore(fixture.ContextFactory));

    private const string BaselineNumber = "ORD-1";
    private static readonly DateOnly BaselineEffectiveDate = new(2026, 7, 10);
    private const EStatus BaselineStatus = EStatus.New;

    // --- Success path ---------------------------------------------------------

    [Fact]
    public async Task Test_Update_Get_Success()
    {
        var order = await CreateBaselineAsync();

        var newEffectiveDate = new DateOnly(2026, 7, 11);
        var maybeUpdated = await _updateService.HandleAsync(new Contracts.Commands.UpdateOrderCommand(
            order.OrderId, "ORD-2", newEffectiveDate, EStatus.InProgress));

        Assert.True(maybeUpdated.IsSucc);
        var updated = maybeUpdated.ToOption().ValueUnsafe();
        Assert.NotNull(updated);
        Assert.Equal(order.OrderId, updated.Order.OrderId);
        Assert.Equal("ORD-2", updated.Order.OrderNumber);
        Assert.Equal(newEffectiveDate, updated.Order.EffectiveDate);
        Assert.Equal(EStatus.InProgress, updated.Order.Status);

        // Persisted (round-trips through a fresh Get)
        var maybeOrder = await _getService.HandleAsync(new Contracts.Queries.GetOrderQuery(order.OrderId));
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

        var actual = await _updateService.HandleAsync(new Contracts.Commands.UpdateOrderCommand(
            missingId.Value, null, null, EStatus.InProgress));

        Assert.True(actual.IsFail);
        var error = Assert.IsType<NotFoundError>(actual.Match(Succ: _ => null!, Fail: e => e));
        Assert.Equal(ErrorCodes.NotFound, error.ErrorCode);
        Assert.Equal(NotFoundError.ForEntity(missingId).Message, error.Message);
    }

    // --- Single-field updates -------------------------------------------------
    // "null = not updated": every request field except OrderId is nullable, and a
    // null field is left untouched. Each test below sets exactly one field and
    // asserts nothing else moved from its baseline.

    [Fact]
    public async Task Test_Update_OnlyNumber_ChangesNumber_PreservesRest()
    {
        var order = await CreateBaselineAsync();

        var updated = await UpdateAsync(new Contracts.Commands.UpdateOrderCommand(
            order.OrderId, "ORD-2", null, null));

        Assert.Equal("ORD-2", updated.OrderNumber);
        AssertUnchangedExcept(updated, order.OrderId, numberShouldBe: "ORD-2");
    }

    [Fact]
    public async Task Test_Update_OnlyEffectiveDate_ChangesEffectiveDate_PreservesRest()
    {
        var order = await CreateBaselineAsync();
        var newEffectiveDate = new DateOnly(2026, 7, 11);

        var updated = await UpdateAsync(new Contracts.Commands.UpdateOrderCommand(
            order.OrderId, null, newEffectiveDate, null));

        Assert.Equal(newEffectiveDate, updated.EffectiveDate);
        AssertUnchangedExcept(updated, order.OrderId, effectiveDateShouldBe: newEffectiveDate);
    }

    [Fact]
    public async Task Test_Update_OnlyStatus_ChangesStatus_PreservesRest()
    {
        var order = await CreateBaselineAsync();

        var updated = await UpdateAsync(new Contracts.Commands.UpdateOrderCommand(
            order.OrderId, null, null, EStatus.InProgress));

        Assert.Equal(EStatus.InProgress, updated.Status);
        AssertUnchangedExcept(updated, order.OrderId, statusShouldBe: EStatus.InProgress);
    }

    [Fact]
    public async Task Test_Update_AllNullExceptId_IsNoOp()
    {
        var order = await CreateBaselineAsync();

        var updated = await UpdateAsync(new Contracts.Commands.UpdateOrderCommand(
            order.OrderId, null, null, null));

        AssertUnchangedExcept(updated, order.OrderId);
        Assert.Equal(order, updated);
    }

    // --- Status transitions ---------------------------------------------------

    [Fact]
    public async Task Test_Update_InvalidTransition_ReturnsFail_AndKeepsStatus()
    {
        var order = await CreateBaselineAsync();

        var result = await _updateService.HandleAsync(new Contracts.Commands.UpdateOrderCommand(
            order.OrderId, null, null, EStatus.Completed));

        Assert.True(result.IsFail);
        var maybeOrder = await _getService.HandleAsync(new Contracts.Queries.GetOrderQuery(order.OrderId));
        Assert.Equal(EStatus.New, maybeOrder.ToOption().ValueUnsafe()!.Order.Status);
    }

    [Fact]
    public async Task Test_Update_SameStatus_IsNoOp()
    {
        var order = await CreateBaselineAsync();

        var updated = await UpdateAsync(new Contracts.Commands.UpdateOrderCommand(
            order.OrderId, null, null, EStatus.New));

        Assert.Equal(order, updated);
    }

    [Fact]
    public async Task Test_Update_StatusWalk_StartThenComplete_Succeeds()
    {
        var order = await CreateBaselineAsync();

        await UpdateAsync(new Contracts.Commands.UpdateOrderCommand(order.OrderId, null, null, EStatus.InProgress));
        var completed = await UpdateAsync(new Contracts.Commands.UpdateOrderCommand(order.OrderId, null, null, EStatus.Completed));

        Assert.Equal(EStatus.Completed, completed.Status);
    }

    // --- Helpers --------------------------------------------------------------

    private async Task<Contracts.Dto.Order> CreateBaselineAsync()
    {
        var maybeCreated = await _createService.HandleAsync(
            new Contracts.Commands.CreateOrderCommand(BaselineNumber, BaselineEffectiveDate));

        Assert.True(maybeCreated.IsSucc);
        var created = maybeCreated.ToOption().ValueUnsafe();
        Assert.NotNull(created);
        return created.Order;
    }

    private async Task<Contracts.Dto.Order> UpdateAsync(Contracts.Commands.UpdateOrderCommand request)
    {
        var maybeUpdated = await _updateService.HandleAsync(request);
        Assert.True(maybeUpdated.IsSucc);
        var updated = maybeUpdated.ToOption().ValueUnsafe();
        Assert.NotNull(updated);
        return updated.Order;
    }

    // Asserts every field except the one(s) explicitly overridden still holds baseline.
    private static void AssertUnchangedExcept(
        Contracts.Dto.Order updated,
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
