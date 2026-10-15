using Acme.Core.BuildingBlocks.Domain;

namespace Acme.Core.Ordering.Application.Orders.Mappers;

public readonly struct StatusMapper : IMap<Domain.Orders.OrderStatus, Contracts.Dto.EStatus>
{
    public Domain.Orders.OrderStatus Map(Contracts.Dto.EStatus right)
    {
        return right switch
        {
            Contracts.Dto.EStatus.New => Domain.Orders.OrderStatus.New,
            Contracts.Dto.EStatus.InProgress => Domain.Orders.OrderStatus.InProgress,
            Contracts.Dto.EStatus.Completed => Domain.Orders.OrderStatus.Completed,
            Contracts.Dto.EStatus.Cancelled => Domain.Orders.OrderStatus.Cancelled,
            Contracts.Dto.EStatus.Rejected => Domain.Orders.OrderStatus.Rejected,
            _ => throw new ArgumentOutOfRangeException(nameof(right), right, null)
        };
    }

    public Contracts.Dto.EStatus Map(Domain.Orders.OrderStatus left)
    {
        return left switch
        {
            Domain.Orders.OrderStatus.New => Contracts.Dto.EStatus.New,
            Domain.Orders.OrderStatus.InProgress => Contracts.Dto.EStatus.InProgress,
            Domain.Orders.OrderStatus.Completed => Contracts.Dto.EStatus.Completed,
            Domain.Orders.OrderStatus.Cancelled => Contracts.Dto.EStatus.Cancelled,
            Domain.Orders.OrderStatus.Rejected => Contracts.Dto.EStatus.Rejected,
            _ => throw new ArgumentOutOfRangeException(nameof(left), left, null)
        };
    }
}
