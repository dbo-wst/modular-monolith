using System.Collections.Immutable;
using Acme.Core.Ordering.Contracts.Dto;

namespace Acme.Core.Ordering.Contracts.Results;

public sealed record GetListOrderItemResult(ImmutableList<OrderItem> OrderItems);
