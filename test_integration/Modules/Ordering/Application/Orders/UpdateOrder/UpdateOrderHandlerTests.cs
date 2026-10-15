using Acme.Core.BuildingBlocks.Domain.Errors;
using LanguageExt.UnsafeValueAccess;
using Acme.Core.Ordering.Persistence;
using Acme.Core.Ordering.TestCompanion;
using CreateOrderHandler = Acme.Core.Ordering.Application.Orders.CreateOrder.CreateOrderHandler;
using Dto = Acme.Core.Ordering.Contracts.Dto;
using EStatus = Acme.Core.Ordering.Contracts.Dto.EStatus;
using GetOrderHandler = Acme.Core.Ordering.Application.Orders.GetOrder.GetOrderHandler;
using OrderId = Acme.Core.Ordering.Domain.Orders.OrderId;
using UpdateOrderHandler = Acme.Core.Ordering.Application.Orders.UpdateOrder.UpdateOrderHandler;

namespace Acme.Core.Ordering.Application.IntegrationTest.Orders.UpdateOrder;

[Collection(IntegrationTestsFixture.CollectionName)]
public class UpdateOrderHandlerTests(IntegrationTestsFixture fixture)
{
    private readonly CreateOrderHandler _createService = OrderingPersistence.Create(fixture.GetRequiredService<IOrderingDbContextFactory>()).ForOrders((r, u) => new CreateOrderHandler(r, u));
    private readonly UpdateOrderHandler _updateService = OrderingPersistence.Create(fixture.GetRequiredService<IOrderingDbContextFactory>()).ForOrders((r, u) => new UpdateOrderHandler(r, u));
    private readonly GetOrderHandler _getService = new(new ReadStore(fixture.GetRequiredService<IOrderingDbContextFactory>()));

    private async Task<Dto.Order> CreateBaselineAsync()
    {
        var maybeCreated = await _createService.HandleAsync(new Contracts.Commands.CreateOrderCommand(
            "ORD-1", new DateOnly(2026, 7, 10)));
        Assert.True(maybeCreated.IsSucc);
        var created = maybeCreated.ToOption().ValueUnsafe();
        Assert.NotNull(created);
        return created.Order;
    }

    private async Task<Dto.Order> UpdateAsync(Contracts.Commands.UpdateOrderCommand request)
    {
        var maybeUpdated = await _updateService.HandleAsync(request);
        Assert.True(maybeUpdated.IsSucc);
        var updated = maybeUpdated.ToOption().ValueUnsafe();
        Assert.NotNull(updated);
        return updated.Order;
    }

    private async Task AssertPersistedAsync(Dto.Order expected)
    {
        var maybeOrder = await _getService.HandleAsync(new Contracts.Queries.GetOrderQuery(expected.OrderId));
        Assert.True(maybeOrder.IsSucc);
        var actual = maybeOrder.ToOption().ValueUnsafe();
        Assert.NotNull(actual);
        Assert.Equal(expected, actual.Order);
    }

    [Fact]
    public async Task Test_Update_Get_Success()
    {
        var baseline = await CreateBaselineAsync();

        var newEffectiveDate = new DateOnly(2026, 7, 11);
        var updated = await UpdateAsync(new Contracts.Commands.UpdateOrderCommand(
            baseline.OrderId, "ORD-2", newEffectiveDate, EStatus.InProgress));

        Assert.Equal(baseline.OrderId, updated.OrderId);
        Assert.Equal("ORD-2", updated.OrderNumber);
        Assert.Equal(newEffectiveDate, updated.EffectiveDate);
        Assert.Equal(EStatus.InProgress, updated.Status);

        await AssertPersistedAsync(updated);
    }

    [Fact]
    public async Task Test_Update_MissingEntity_ReturnsFail()
    {
        var missingId = OrderId.From(Guid.NewGuid());

        var actual = await _updateService.HandleAsync(new Contracts.Commands.UpdateOrderCommand(
            missingId.Value, "ORD-2", null, EStatus.InProgress));

        Assert.True(actual.IsFail);
        var error = Assert.IsType<NotFoundError>(actual.Match(Succ: _ => null!, Fail: e => e));
        Assert.Equal(ErrorCodes.NotFound, error.ErrorCode);
        Assert.Equal(NotFoundError.ForEntity(missingId).Message, error.Message);
    }

    [Fact]
    public async Task Test_Update_OnlyNumber_ChangesOnlyNumber()
    {
        var baseline = await CreateBaselineAsync();

        var updated = await UpdateAsync(new Contracts.Commands.UpdateOrderCommand(
            baseline.OrderId, "ORD-CHANGED", null, null));

        Assert.Equal(baseline with { OrderNumber = "ORD-CHANGED" }, updated);
        await AssertPersistedAsync(updated);
    }

    [Fact]
    public async Task Test_Update_OnlyEffectiveDate_ChangesOnlyEffectiveDate()
    {
        var baseline = await CreateBaselineAsync();
        var newEffectiveDate = new DateOnly(2026, 7, 11);

        var updated = await UpdateAsync(new Contracts.Commands.UpdateOrderCommand(
            baseline.OrderId, null, newEffectiveDate, null));

        Assert.Equal(baseline with { EffectiveDate = newEffectiveDate }, updated);
        await AssertPersistedAsync(updated);
    }

    [Fact]
    public async Task Test_Update_OnlyStatus_ChangesOnlyStatus()
    {
        var baseline = await CreateBaselineAsync();

        var updated = await UpdateAsync(new Contracts.Commands.UpdateOrderCommand(
            baseline.OrderId, null, null, EStatus.InProgress));

        Assert.Equal(baseline with { Status = EStatus.InProgress }, updated);
        await AssertPersistedAsync(updated);
    }

    [Fact]
    public async Task Test_Update_AllNullMutableFields_PreservesEntity()
    {
        var baseline = await CreateBaselineAsync();

        var updated = await UpdateAsync(new Contracts.Commands.UpdateOrderCommand(
            baseline.OrderId, null, null, null));

        Assert.Equal(baseline, updated);
        await AssertPersistedAsync(updated);
    }
}
