using LanguageExt;

namespace Acme.Core.BuildingBlocks.Domain;

/// <summary>
/// Domain-level port for committing the changes of one business operation. A successful
/// <see cref="SaveChangesAsync"/> also dispatches the domain events of every aggregate touched in the scope.
/// </summary>
public interface IUnitOfWork
{
    Task<Fin<Unit>> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<Fin<T>> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<Fin<T>>> work,
        CancellationToken cancellationToken = default);
}
