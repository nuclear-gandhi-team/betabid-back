using Betabid.Application.DTOs.FilteringDto;
using Betabid.Domain.Entities;
using Betabid.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Betabid.Persistence.Extensions;

public static class SortingExtension
{
    public static Task<IQueryable<Lot>> GetSorted(this IQueryable<Lot> lots, FilteringOptionsDto filteringOptions)
    {
        lots = lots.Include(l => l.Bets);
        
        lots = filteringOptions.PriceOrder switch
        {
            SortingOptions.Asc => lots.OrderBy(lot => lot.Bets.Any() ? lot.Bets.Max(bet => bet.Amount) : lot.StartPrice),
            SortingOptions.Desc => lots.OrderByDescending(lot => lot.Bets.Any() ? lot.Bets.Max(bet => bet.Amount) : lot.StartPrice),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        return Task.FromResult(lots);
    }
}