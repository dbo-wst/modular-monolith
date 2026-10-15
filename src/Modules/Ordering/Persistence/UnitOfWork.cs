using Acme.Core.BuildingBlocks.Domain;
using Acme.Core.BuildingBlocks.Persistence;
using Acme.Core.Ordering.Persistence.DbContext;

namespace Acme.Core.Ordering.Persistence;

public sealed class UnitOfWork(
    OrderingDbContext context,
    DomainEventCollector collector,
    IDomainEventDispatcher dispatcher)
    : UnitOfWork<OrderingDbContext>(context, collector, dispatcher);
