using System.Collections.Immutable;
using Template.Core.Ordering.Contracts.Dto;

namespace Template.Core.Ordering.Contracts.Results;

public sealed record GetListOrderItemResult(ImmutableList<OrderItem> OrderItems);
