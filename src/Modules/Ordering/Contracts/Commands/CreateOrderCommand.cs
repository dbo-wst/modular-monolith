using Template.Core.Ordering.Contracts.Results;
using Template.Core.BuildingBlocks.Application.Communication;

namespace Template.Core.Ordering.Contracts.Commands;

public sealed record CreateOrderCommand(
    string? Number,
    DateOnly? EffectiveDate) : ICommand<CreateOrderResult>;
