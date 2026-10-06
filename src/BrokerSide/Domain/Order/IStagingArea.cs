using LanguageExt;

namespace Wst.Trios.Core.BrokerSide.Domain.Order;

public interface IStagingArea
{
    Task<Fin<Order>> AddAsync(Order order, CancellationToken cancellationToken = default);
    Task<Fin<Order>> UpdateAsync(Order order, CancellationToken cancellationToken = default);
    Task<Fin<Unit>> DeleteAsync(OrderId orderId, CancellationToken cancellationToken = default);
    Task<Fin<Order>> GetByIdAsync(OrderId orderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetListAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
