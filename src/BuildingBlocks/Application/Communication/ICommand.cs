using LanguageExt;

namespace Acme.Core.BuildingBlocks.Application.Communication;

/// <summary>A request that changes state.</summary>
public interface ICommand<TResponse> : IRequest<TResponse>;

/// <summary>A request that changes state and returns no value (<see cref="Unit"/>).</summary>
public interface ICommand : ICommand<Unit>;
