namespace Template.Core.Shared.Kernel.Communication;

/// <summary>A request that changes state.</summary>
public interface ICommand<TResponse> : IRequest<TResponse>;
