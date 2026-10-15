using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Acme.Core.BuildingBlocks.Domain;
using Acme.Core.BuildingBlocks.Domain.Errors;

namespace Acme.Core.BuildingBlocks.Persistence;

/// <summary>
/// Reusable EF Core unit of work: saves changes and runs work inside a transaction (honouring the
/// context's execution strategy), mapping infrastructure exceptions to expected <see cref="Fin{A}"/> errors.
/// After a successful save the domain events of every aggregate tracked by the scoped
/// <see cref="DomainEventCollector"/> are dispatched in-process (same scope) and cleared; a failed save
/// dispatches nothing. Modules derive from it to bind their DbContext.
/// </summary>
public abstract class UnitOfWork<TContext>(
    TContext context,
    DomainEventCollector collector,
    IDomainEventDispatcher dispatcher) : IUnitOfWork
    where TContext : DbContext
{
    protected TContext Context { get; } = context;

    public async Task<Fin<Unit>> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await Context.SaveChangesAsync(cancellationToken);
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

        return await dispatcher.DispatchAsync(collector.Drain(), cancellationToken);
    }

    public async Task<Fin<TResult>> ExecuteInTransactionAsync<TResult>(
        Func<CancellationToken, Task<Fin<TResult>>> work,
        CancellationToken cancellationToken = default)
    {
        var strategy = Context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Context.Database.BeginTransactionAsync(cancellationToken);

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
