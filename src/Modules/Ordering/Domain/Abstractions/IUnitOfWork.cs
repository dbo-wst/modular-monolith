using LanguageExt;

namespace Template.Core.Ordering.Domain.Abstractions;

public interface IUnitOfWork
{
    Task<Fin<Unit>> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<Fin<T>> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<Fin<T>>> work,
        CancellationToken cancellationToken = default);
}
