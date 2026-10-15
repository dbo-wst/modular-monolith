using Acme.Core.Ordering.Contracts.Results;
using Acme.Core.BuildingBlocks.Application.Communication;

namespace Acme.Core.Ordering.Contracts.Commands;

public sealed record CreateOrderCommand(
    string? Number,
    DateOnly? EffectiveDate) : ICommand<CreateOrderResult>;
