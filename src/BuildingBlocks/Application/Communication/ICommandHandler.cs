using LanguageExt;

namespace Acme.Core.BuildingBlocks.Application.Communication;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>;

/// <summary>Handler for a <see cref="ICommand"/> that returns no value.</summary>
public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, Unit>
    where TCommand : ICommand;
