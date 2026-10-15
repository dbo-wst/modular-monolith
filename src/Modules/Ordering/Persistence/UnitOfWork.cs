using Template.Core.BuildingBlocks.Domain;
using Template.Core.BuildingBlocks.Persistence;
using Template.Core.Ordering.Persistence.DbContext;

namespace Template.Core.Ordering.Persistence;

public sealed class UnitOfWork(
    OrderingDbContext context,
    DomainEventCollector collector,
    IDomainEventDispatcher dispatcher)
    : UnitOfWork<OrderingDbContext>(context, collector, dispatcher);
