using LanguageExt;

namespace Acme.Core.BuildingBlocks.Application.Communication;

/// <summary>Invokes the next step of the mediator pipeline (the next behaviour, or finally the handler).</summary>
public delegate Task<Fin<TResponse>> RequestHandlerDelegate<TResponse>();
