using LanguageExt;

namespace Template.Core.Ordering.Domain.Orders;

public interface IOrderRepository
{
    Task<Fin<Order>> AddAsync(Order order, CancellationToken cancellationToken = default);
    Task<Fin<Order>> UpdateAsync(Order order, CancellationToken cancellationToken = default);
    Task<Fin<Unit>> DeleteAsync(OrderId orderId, CancellationToken cancellationToken = default);
    Task<Fin<Order>> GetByIdAsync(OrderId orderId, CancellationToken cancellationToken = default);
}
