using LanguageExt;
using LanguageExt.Common;
using Wst.Trios.Core.BrokerSide.Domain.Order;
using Wst.Trios.Core.BrokerSide.TestCompanion;
using Wst.Trios.Core.Shared.Kernel.Domain;
using Wst.Trios.Core.Shared.Kernel.Errors;
using Wst.Trios.Core.Shared.Kernel.Functional;

namespace Wst.Trios.Core.BrokerSide.Persistence.IntegrationTest;

[Collection(IntegrationTestsFixture.CollectionName)]
public class StoreTests(IntegrationTestsFixture fixture)
{
    private readonly IBrokerSideDbContextFactory _contextFactory =
        fixture.GetRequiredService<IBrokerSideDbContextFactory>();

    private static Domain.Order.Order NewOrder() => new(
        OrderId.From(Guid.NewGuid()), BrokerId.From(Guid.NewGuid()), InstrumentId.From(Guid.NewGuid()), VenueId.From(Guid.NewGuid()), OrderNumber.From("ORD-1"), EffectiveDate.From(new DateOnly(2026, 7, 10)), Quantity.From(100.5m), Price.From(25.75m), Side.From(ESide.Buy), Status.From(EStatus.New));

    [Fact]
    public async Task SaveChangesAsync_PersistsChangesAndReturnsSucc()
    {
        var order = NewOrder();
        await using var store = new Store(_contextFactory);

        await store.Orders.AddAsync(order);
        var result = await store.SaveChangesAsync();

        Assert.True(result.IsSucc);

        await using var verifyStore = new Store(_contextFactory);
        var fetched = await verifyStore.Orders.GetByIdAsync(order.OrderId);
        Assert.True(fetched.IsSucc);
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WhenWorkSucceeds_CommitsChanges()
    {
        var order = NewOrder();
        await using var store = new Store(_contextFactory);

        var result = await store.ExecuteInTransactionAsync(async ct =>
        {
            await store.Orders.AddAsync(order, ct);
            return await store.SaveChangesAsync(ct);
        });

        Assert.True(result.IsSucc);

        await using var verifyStore = new Store(_contextFactory);
        var fetched = await verifyStore.Orders.GetByIdAsync(order.OrderId);
        Assert.True(fetched.IsSucc);
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WhenWorkFails_RollsBackChanges()
    {
        var order = NewOrder();
        var failure = Error.New("forced failure");
        await using var store = new Store(_contextFactory);

        var result = await store.ExecuteInTransactionAsync(async ct =>
        {
            await store.Orders.AddAsync(order, ct);
            await store.SaveChangesAsync(ct);
            return Fin<Unit>.Fail(failure);
        });

        Assert.True(result.IsFail);

        await using var verifyStore = new Store(_contextFactory);
        var fetched = await verifyStore.Orders.GetByIdAsync(order.OrderId);
        Assert.True(fetched.IsFail);
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WhenWorkSucceeds_ReturnsWorkValue()
    {
        await using var store = new Store(_contextFactory);

        var result = await store.ExecuteInTransactionAsync(_ => Fin<int>.Succ(42).AsTask());

        Assert.True(result.IsSucc);
        result.IfSucc(value => Assert.Equal(42, value));
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WhenWorkFails_ReturnsWorkError()
    {
        var failure = Error.New("work failed");
        await using var store = new Store(_contextFactory);

        var result = await store.ExecuteInTransactionAsync(_ => Fin<int>.Fail(failure).AsTask());

        Assert.True(result.IsFail);
        result.IfFail(error => Assert.Equal("work failed", error.Message));

    }


    [Fact]
    public async Task ExecuteInTransactionAsync_WhenWorkFails_WorkThrowsException()
    {
        await using var store = new Store(_contextFactory);

        var result = await store.ExecuteInTransactionAsync(_ =>
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

        await using var store = new Store(_contextFactory);
        await store.Orders.AddAsync(order);

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var result = await store.SaveChangesAsync(cts.Token);

        Assert.True(result.IsFail);
        result.IfFail(error => Assert.Equal(OperationCancelled.ErrorMessage, error.Message));
    }

    [Fact]
    public async Task SaveChangesAsync_WhenConcurrencyConflict_ReturnsConcurrencyError()
    {
        var order = NewOrder();
        await using var seedStore = new Store(_contextFactory);
        await seedStore.Orders.AddAsync(order);
        await seedStore.SaveChangesAsync();

        // Two stores load the same row (same RowVersion) before either persists.
        await using var store1 = new Store(_contextFactory);
        await using var store2 = new Store(_contextFactory);
        await store1.Orders.UpdateAsync(order with { Quantity = Quantity.From(200m) });
        await store2.Orders.UpdateAsync(order with { Quantity = Quantity.From(300m) });

        var first = await store1.SaveChangesAsync();
        Assert.True(first.IsSucc);

        // store2 still holds the stale RowVersion, so its update conflicts.
        var second = await store2.SaveChangesAsync();

        Assert.True(second.IsFail);
        second.IfFail(error => Assert.Equal(ConcurrencyConflict.ErrorMessage, error.Message));
    }

    [Fact]
    public async Task SaveChangesAsync_WhenInsertViolatesPrimaryKey_ReturnsPersistenceError()
    {
        var order = NewOrder();

        // Both stores stage an insert for the same OrderId before either persists,
        // so neither sees the other's row during the existence check.
        await using var store1 = new Store(_contextFactory);
        await using var store2 = new Store(_contextFactory);
        await store1.Orders.AddAsync(order);
        await store2.Orders.AddAsync(order);

        var first = await store1.SaveChangesAsync();
        Assert.True(first.IsSucc);

        // store2's insert hits the duplicate primary key.
        var second = await store2.SaveChangesAsync();

        Assert.True(second.IsFail);
        second.IfFail(error => Assert.Equal(PersistenceFailed.ErrorMessage, error.Message));
    }
}
