namespace Acme.Core.BuildingBlocks.Application.Communication;

/// <summary>A request that only reads state.</summary>
public interface IQuery<TResponse> : IRequest<TResponse>;
