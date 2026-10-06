using Wst.Trios.Core.BrokerSide.Integration.Responses;
using Wst.Trios.Core.Shared.Kernel.Communication;

namespace Wst.Trios.Core.BrokerSide.Integration.Requests;

public sealed record GetListOrderRequest(int PageNumber, int PageSize): IRequest<GetListOrderResponse>;


