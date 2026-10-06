using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Wst.Trios.Core.Shared.Kernel.Errors;

namespace Wst.Trios.Core.Shared.Kernel.Persistence;

public static class StoreFunc
{

    public static async Task<Fin<Unit>> SaveChangesAsync<TContext>(this TContext that, CancellationToken cancellationToken = default)
        where TContext: DbContext
    {
        try
        {
            await that.SaveChangesAsync(cancellationToken);
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

    public static async Task<Fin<TResult>> ExecuteInTransactionAsync<TContext, TResult>(this TContext that,
        Func<CancellationToken, Task<Fin<TResult>>> work,
        CancellationToken cancellationToken = default)
            where TContext: DbContext
    {
        var strategy = that.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await that.Database.BeginTransactionAsync(cancellationToken);

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
