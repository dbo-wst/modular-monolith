using LanguageExt;
using LanguageExt.Common;
using Acme.Core.BuildingBlocks.Domain;

namespace Acme.Core.Ordering.TestCompanion;

/// <summary>Test double that records every dispatched event; optionally fails every dispatch.</summary>
public sealed class RecordingDomainEventDispatcher(Error? failWith = null) : IDomainEventDispatcher
{
    private readonly List<DomainEvent> _dispatched = [];

    public IReadOnlyList<DomainEvent> Dispatched => _dispatched;

    public Task<Fin<Unit>> DispatchAsync(IEnumerable<DomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        _dispatched.AddRange(domainEvents);
        return Task.FromResult(failWith is null ? Fin<Unit>.Succ(Unit.Default) : Fin<Unit>.Fail(failWith));
    }
}
