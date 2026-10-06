using LanguageExt;

namespace Wst.Trios.Core.BrokerSide.Domain;

public interface IStore : IAsyncDisposable
{
    Order.IStagingArea Orders { get; }
    Trade.IStagingArea Trades { get; }

    Task<Fin<Unit>> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<Fin<T>> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<Fin<T>>> work,
        CancellationToken cancellationToken = default);
}
