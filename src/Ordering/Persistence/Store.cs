using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Template.Core.Ordering.Persistence.DbContext;
using Template.Core.Shared.Kernel.Errors;

namespace Template.Core.Ordering.Persistence;

public sealed class Store : Domain.IStore
{
    private readonly OrderingDbContext _context;
    public Domain.Order.IStagingArea Orders { get; }
    public Domain.OrderItem.IStagingArea OrderItems { get; }

    public Store(IOrderingDbContextFactory contextFactory)
    {
        _context = contextFactory.CreateDbContext();
        Orders = new Order.StagingArea(_context);
        OrderItems = new OrderItem.StagingArea(_context);

    }

    public async Task<Fin<Unit>> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (OperationCanceledException exception)
        {
            return OperationCancelled.New(exception);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            return ConcurrencyConflict.New(exception);
        }
        catch (DbUpdateException exception)
        {
            return PersistenceFailed.New(exception);
        }

        return Fin<Unit>.Succ(Unit.Default);
    }

    public async Task<Fin<T>> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<Fin<T>>> work,
        CancellationToken cancellationToken = default)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var result = await work(cancellationToken);

                if (result.IsSucc)
                    await transaction.CommitAsync(cancellationToken);
                else
                    await transaction.RollbackAsync(cancellationToken);

                return result;
            }
            catch (Exception exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                return TransactionFailed.New(exception);
            }
        });
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}
