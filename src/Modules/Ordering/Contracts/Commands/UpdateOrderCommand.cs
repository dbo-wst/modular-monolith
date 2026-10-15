using Template.Core.Ordering.Contracts.Dto;
using Template.Core.Ordering.Contracts.Results;
using Template.Core.Shared.Kernel.Communication;

namespace Template.Core.Ordering.Contracts.Commands;

public sealed record UpdateOrderCommand(
    Guid OrderId,
    string? Number,
    DateOnly? EffectiveDate,
    EStatus? Status) : ICommand<UpdateOrderResult>;
