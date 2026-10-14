using LanguageExt;

namespace Template.Core.Ordering.Domain.OrderItem;

public interface IStagingArea
{
    Task<Fin<OrderItem>> AddAsync(OrderItem orderItem, CancellationToken cancellationToken = default);
    Task<Fin<OrderItem>> UpdateAsync(OrderItem orderItem, CancellationToken cancellationToken = default);
    Task<Fin<Unit>> DeleteAsync(OrderItemId orderItemId, CancellationToken cancellationToken = default);
    Task<Fin<OrderItem>> GetByIdAsync(OrderItemId orderItemId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderItem>> GetListAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
