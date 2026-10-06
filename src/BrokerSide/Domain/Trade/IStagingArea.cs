using LanguageExt;

namespace Wst.Trios.Core.BrokerSide.Domain.Trade;

public interface IStagingArea
{
    Task<Fin<Trade>> AddAsync(Trade trade, CancellationToken cancellationToken = default);
    Task<Fin<Trade>> UpdateAsync(Trade trade, CancellationToken cancellationToken = default);
    Task<Fin<Unit>> DeleteAsync(TradeId tradeId, CancellationToken cancellationToken = default);
    Task<Fin<Trade>> GetByIdAsync(TradeId tradeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Trade>> GetListAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
