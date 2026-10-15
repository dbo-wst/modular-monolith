using LanguageExt;
using Template.Core.Ordering.Contracts.Dto;

namespace Template.Core.Ordering.Application;

/// <summary>Query-side port: returns read models (DTOs) directly, bypassing the domain aggregates.</summary>
public interface IOrderingReadStore
{
    Task<Fin<Order>> GetOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetOrdersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<Fin<OrderItem>> GetOrderItemAsync(Guid orderItemId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderItem>> GetOrderItemsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
