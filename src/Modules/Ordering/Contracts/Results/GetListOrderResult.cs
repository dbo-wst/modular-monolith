using System.Collections.Immutable;
using Acme.Core.Ordering.Contracts.Dto;

namespace Acme.Core.Ordering.Contracts.Results;

public sealed record GetListOrderResult(ImmutableList<Order> Orders);
