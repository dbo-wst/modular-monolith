using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Template.Core.Ordering.Infrastructure.DbContext;
using Template.Core.Shared.Kernel.Errors;

namespace Template.Core.Ordering.Infrastructure;

public sealed class UnitOfWork(OrderingDbContext context) : Domain.Abstractions.IUnitOfWork
{
    public async Task<Fin<Unit>> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await context.SaveChangesAsync(cancellationToken);
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
        var strategy = context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

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
}
