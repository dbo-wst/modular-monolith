using LanguageExt;

namespace Template.Core.Ordering.Domain;

public interface IStore : IAsyncDisposable
{
    Order.IStagingArea Orders { get; }
    OrderItem.IStagingArea OrderItems { get; }

    Task<Fin<Unit>> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<Fin<T>> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<Fin<T>>> work,
        CancellationToken cancellationToken = default);
}
