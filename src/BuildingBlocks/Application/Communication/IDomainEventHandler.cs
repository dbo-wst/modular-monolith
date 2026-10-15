using LanguageExt;
using Template.Core.BuildingBlocks.Domain;

namespace Template.Core.BuildingBlocks.Application.Communication;

/// <summary>
/// Reacts in-process to a domain event after the unit of work saved the change that raised it.
/// An event type may have any number of handlers (including none).
/// </summary>
public interface IDomainEventHandler<in TEvent>
    where TEvent : DomainEvent
{
    Task<Fin<Unit>> HandleAsync(TEvent domainEvent, CancellationToken ct = default);
}
