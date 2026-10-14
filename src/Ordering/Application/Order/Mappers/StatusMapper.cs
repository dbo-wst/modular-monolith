using Template.Core.Shared.Kernel;

namespace Template.Core.Ordering.Application.Order.Mappers;

public readonly struct StatusMapper : IMap<Domain.Order.EStatus, Integration.Dto.EStatus>
{
    public Domain.Order.EStatus Map(Integration.Dto.EStatus right)
    {
        return right switch
        {
            Integration.Dto.EStatus.New => Domain.Order.EStatus.New,
            Integration.Dto.EStatus.InProgress => Domain.Order.EStatus.InProgress,
            Integration.Dto.EStatus.Completed => Domain.Order.EStatus.Completed,
            Integration.Dto.EStatus.Cancelled => Domain.Order.EStatus.Cancelled,
            Integration.Dto.EStatus.Rejected => Domain.Order.EStatus.Rejected,
            _ => throw new ArgumentOutOfRangeException(nameof(right), right, null)
        };
    }

    public Integration.Dto.EStatus Map(Domain.Order.EStatus left)
    {
        return left switch
        {
            Domain.Order.EStatus.New => Integration.Dto.EStatus.New,
            Domain.Order.EStatus.InProgress => Integration.Dto.EStatus.InProgress,
            Domain.Order.EStatus.Completed => Integration.Dto.EStatus.Completed,
            Domain.Order.EStatus.Cancelled => Integration.Dto.EStatus.Cancelled,
            Domain.Order.EStatus.Rejected => Integration.Dto.EStatus.Rejected,
            _ => throw new ArgumentOutOfRangeException(nameof(left), left, null)
        };
    }
}
