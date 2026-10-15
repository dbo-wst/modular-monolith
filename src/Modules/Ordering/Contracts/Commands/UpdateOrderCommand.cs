using Acme.Core.Ordering.Contracts.Dto;
using Acme.Core.Ordering.Contracts.Results;
using Acme.Core.BuildingBlocks.Application.Communication;

namespace Acme.Core.Ordering.Contracts.Commands;

public sealed record UpdateOrderCommand(
    Guid OrderId,
    string? Number,
    DateOnly? EffectiveDate,
    EStatus? Status) : ICommand<UpdateOrderResult>;
