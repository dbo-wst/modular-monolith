namespace Template.Core.BuildingBlocks.Application.Communication;

/// <summary>
/// Marker for a request dispatched by <see cref="IMediator"/> that yields <typeparamref name="TResponse"/>.
/// Covariant so a request can be treated as a request of a less derived response type.
/// </summary>
public interface IRequest<out TResponse>;
