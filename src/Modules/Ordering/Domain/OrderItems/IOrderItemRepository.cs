using LanguageExt;

namespace Template.Core.Ordering.Domain.OrderItems;

public interface IOrderItemRepository
{
    Task<Fin<OrderItem>> AddAsync(OrderItem orderItem, CancellationToken cancellationToken = default);
    Task<Fin<OrderItem>> UpdateAsync(OrderItem orderItem, CancellationToken cancellationToken = default);
    Task<Fin<Unit>> DeleteAsync(OrderItem orderItem, CancellationToken cancellationToken = default);
    Task<Fin<OrderItem>> GetByIdAsync(OrderItemId orderItemId, CancellationToken cancellationToken = default);
}
