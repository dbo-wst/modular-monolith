using Wst.Trios.Core.BrokerSide.Integration.Responses;
using Wst.Trios.Core.Shared.Kernel.Communication;

namespace Wst.Trios.Core.BrokerSide.Integration.Requests;

public sealed record DeleteOrderRequest(Guid OrderId): IRequest<DeleteOrderResponse>;


