using System.Collections.Immutable;
using Template.Core.Ordering.Integration.Dto;

namespace Template.Core.Ordering.Integration.Responses;

public sealed record GetListOrderItemResponse(ImmutableList<OrderItem> OrderItems);
