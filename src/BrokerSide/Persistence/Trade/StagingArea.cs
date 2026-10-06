using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Wst.Trios.Core.BrokerSide.Persistence.DbContext;
using DomainTrade = Wst.Trios.Core.BrokerSide.Domain.Trade;

namespace Wst.Trios.Core.BrokerSide.Persistence.Trade;

public class StagingArea(BrokerSideDbContext context): DomainTrade.IStagingArea
{
    public async Task<Fin<DomainTrade.Trade>> AddAsync(DomainTrade.Trade trade, CancellationToken cancellationToken = default)
    {
        var alreadyExists = await context.Trades
            .AnyAsync(i => i.TradeId == trade.TradeId.Value, cancellationToken);

        if (alreadyExists)
        {
            return PersistenceErrors.AlreadyExists(trade);
        }

        var mapped = DboMappers.Trade.Map(trade);
        await context.Trades.AddAsync(mapped, cancellationToken);

        return Fin<DomainTrade.Trade>.Succ(trade);
    }

    public async Task<Fin<DomainTrade.Trade>> UpdateAsync(DomainTrade.Trade trade, CancellationToken cancellationToken = default)
    {
        var found = await context.Trades
            .FirstOrDefaultAsync(i => i.TradeId == trade.TradeId.Value, cancellationToken);

        if (found == null)
        {
            return PersistenceErrors.NotFound(trade);
        }

        var mapped = DboMappers.Trade.Map(trade);

        context.Entry(found).CurrentValues.SetValues(mapped);

        return Fin<DomainTrade.Trade>.Succ(trade);
    }

    public async Task<Fin<Unit>> DeleteAsync(DomainTrade.TradeId tradeId, CancellationToken cancellationToken = default)
    {
        var found = await context.Trades
            .FirstOrDefaultAsync(i => i.TradeId == tradeId.Value, cancellationToken);

        if (found == null)
        {
            return PersistenceErrors.NotFound(tradeId);
        }

        context.Trades.Remove(found);

        return Fin<Unit>.Succ(Unit.Default);
    }

    public async Task<Fin<DomainTrade.Trade>> GetByIdAsync(DomainTrade.TradeId tradeId, CancellationToken cancellationToken = default)
    {
        var found = await context.Trades
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.TradeId == tradeId.Value, cancellationToken);

        return found == null
            ? PersistenceErrors.NotFound(tradeId)
            : DboMappers.Trade.Map(found);
    }


    public async Task<IReadOnlyList<DomainTrade.Trade>> GetListAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        const int defaultPageSize = 10;
        const int maxPageSize = 100;
        const int maxPageNumber = 1000;

        var page = Math.Min(maxPageNumber, Math.Max(pageNumber, 1));
        var size = pageSize <= 0 ? defaultPageSize : Math.Min(pageSize, maxPageSize);

        var result = await context.Trades
            .Skip((page - 1) * size)
            .Take(size)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return result.Map(DboMappers.Trade.Map).ToList();
    }
}
