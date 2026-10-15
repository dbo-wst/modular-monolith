using LanguageExt;
using LanguageExt.Common;
using Template.Core.Ordering.Domain.Orders;
using Template.Core.Ordering.TestCompanion;
using Template.Core.Ordering.Domain.ValueObjects;
using Template.Core.BuildingBlocks.Domain.Errors;
using Template.Core.BuildingBlocks.Domain.Functional;

namespace Template.Core.Ordering.Persistence.IntegrationTest;

[Collection(IntegrationTestsFixture.CollectionName)]
public class UnitOfWorkTests(IntegrationTestsFixture fixture)
{
    private readonly IOrderingDbContextFactory _contextFactory =
        fixture.GetRequiredService<IOrderingDbContextFactory>();

    private static Domain.Orders.Order NewOrder() => Domain.Orders.Order.Rehydrate(
        OrderId.From(Guid.NewGuid()), OrderNumber.From("ORD-1"), EffectiveDate.From(new DateOnly(2026, 7, 10)), OrderStatus.New);

    private static Domain.Orders.Order Renumbered(Domain.Orders.Order o, string number) =>
        Domain.Orders.Order.Rehydrate(o.Id, OrderNumber.From(number), o.EffectiveDate, o.Status);

    [Fact]
    public async Task SaveChangesAsync_PersistsChangesAndReturnsSucc()
    {
        var order = NewOrder();
        await using var store = OrderingPersistence.Create(_contextFactory);

        await store.Orders.AddAsync(order);
        var result = await store.UnitOfWork.SaveChangesAsync();

        Assert.True(result.IsSucc);

        await using var verifyStore = OrderingPersistence.Create(_contextFactory);
        var fetched = await verifyStore.Orders.GetByIdAsync(order.Id);
        Assert.True(fetched.IsSucc);
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WhenWorkSucceeds_CommitsChanges()
    {
        var order = NewOrder();
        await using var store = OrderingPersistence.Create(_contextFactory);

        var result = await store.UnitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            await store.Orders.AddAsync(order, ct);
            return await store.UnitOfWork.SaveChangesAsync(ct);
        });

        Assert.True(result.IsSucc);

        await using var verifyStore = OrderingPersistence.Create(_contextFactory);
        var fetched = await verifyStore.Orders.GetByIdAsync(order.Id);
        Assert.True(fetched.IsSucc);
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WhenWorkFails_RollsBackChanges()
    {
        var order = NewOrder();
        var failure = Error.New("forced failure");
        await using var store = OrderingPersistence.Create(_contextFactory);

        var result = await store.UnitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            await store.Orders.AddAsync(order, ct);
            await store.UnitOfWork.SaveChangesAsync(ct);
            return Fin<Unit>.Fail(failure);
        });

        Assert.True(result.IsFail);

        await using var verifyStore = OrderingPersistence.Create(_contextFactory);
        var fetched = await verifyStore.Orders.GetByIdAsync(order.Id);
        Assert.True(fetched.IsFail);
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WhenWorkSucceeds_ReturnsWorkValue()
    {
        await using var store = OrderingPersistence.Create(_contextFactory);

        var result = await store.UnitOfWork.ExecuteInTransactionAsync(_ => Fin<int>.Succ(42).AsTask());

        Assert.True(result.IsSucc);
        result.IfSucc(value => Assert.Equal(42, value));
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WhenWorkFails_ReturnsWorkError()
    {
        var failure = Error.New("work failed");
        await using var store = OrderingPersistence.Create(_contextFactory);

        var result = await store.UnitOfWork.ExecuteInTransactionAsync(_ => Fin<int>.Fail(failure).AsTask());

        Assert.True(result.IsFail);
        result.IfFail(error => Assert.Equal("work failed", error.Message));

    }


    [Fact]
    public async Task ExecuteInTransactionAsync_WhenWorkFails_WorkThrowsException()
    {
        await using var store = OrderingPersistence.Create(_contextFactory);

        var result = await store.UnitOfWork.ExecuteInTransactionAsync(_ =>
        {
            throw new Exception("work failed");

#pragma warning disable CS0162 // Unreachable code detected
            return Unit.Default.ToFinAsync();
#pragma warning restore CS0162 // Unreachable code detected
        });

        Assert.True(result.IsFail);
        result.IfFail(error => Assert.Equal(TransactionFailed.ErrorMessage, error.Message));
    }

    [Fact]
    public async Task SaveChangesAsync_WhenCancelled_ReturnsCancellationError()
    {
        var order = NewOrder();

        await using var store = OrderingPersistence.Create(_contextFactory);
        await store.Orders.AddAsync(order);

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var result = await store.UnitOfWork.SaveChangesAsync(cts.Token);

        Assert.True(result.IsFail);
        result.IfFail(error => Assert.Equal(OperationCancelled.ErrorMessage, error.Message));
    }

    [Fact]
    public async Task SaveChangesAsync_WhenConcurrencyConflict_ReturnsConcurrencyError()
    {
        var order = NewOrder();
        await using var seedStore = OrderingPersistence.Create(_contextFactory);
        await seedStore.Orders.AddAsync(order);
        await seedStore.UnitOfWork.SaveChangesAsync();

        // Two stores load the same row (same RowVersion) before either persists.
        await using var store1 = OrderingPersistence.Create(_contextFactory);
        await using var store2 = OrderingPersistence.Create(_contextFactory);
        await store1.Orders.UpdateAsync(Renumbered(order, "ORD-2"));
        await store2.Orders.UpdateAsync(Renumbered(order, "ORD-3"));

        var first = await store1.UnitOfWork.SaveChangesAsync();
        Assert.True(first.IsSucc);

        // store2 still holds the stale RowVersion, so its update conflicts.
        var second = await store2.UnitOfWork.SaveChangesAsync();

        Assert.True(second.IsFail);
        second.IfFail(error => Assert.Equal(ConcurrencyConflict.ErrorMessage, error.Message));
    }

    [Fact]
    public async Task SaveChangesAsync_WhenInsertViolatesPrimaryKey_ReturnsPersistenceError()
    {
        var order = NewOrder();

        // Both stores stage an insert for the same OrderId before either persists,
        // so neither sees the other's row during the existence check.
        await using var store1 = OrderingPersistence.Create(_contextFactory);
        await using var store2 = OrderingPersistence.Create(_contextFactory);
        await store1.Orders.AddAsync(order);
        await store2.Orders.AddAsync(order);

        var first = await store1.UnitOfWork.SaveChangesAsync();
        Assert.True(first.IsSucc);

        // store2's insert hits the duplicate primary key.
        var second = await store2.UnitOfWork.SaveChangesAsync();

        Assert.True(second.IsFail);
        second.IfFail(error => Assert.Equal(PersistenceFailed.ErrorMessage, error.Message));
    }
}
