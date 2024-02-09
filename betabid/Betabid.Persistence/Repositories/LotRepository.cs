using System.Linq.Expressions;
using Betabid.Application.DTOs.FilteringDto;
using Betabid.Application.Interfaces.Repositories;
using Betabid.Domain.Entities;
using Betabid.Persistence.Context;
using Betabid.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Betabid.Persistence.Repositories;

public class LotRepository : Repository<Lot>, ILotRepository
{
    private readonly DataContext _context;
    
    public LotRepository(DataContext dbContext, DataContext context) : base(dbContext)
    {
        _context = context;
    }
    
    public async Task<bool> IsLotSavedByUserAsync(int lotId, string userId)
    {
        return await _context.Saved.AnyAsync(s => s.LotId == lotId && s.UserId == userId);
    }
    
    public async Task<Lot> GetByIdWithTagsAsync(int id)
    {
        return await _context.Lots
            .Include(lot => lot.Owner)
            .Include(lot => lot.Pictures)
            .Include(lot => lot.Tags)
            .Include(lot => lot.Bets)
            .FirstOrDefaultAsync(lot => lot.Id == id)
               ?? throw new InvalidOperationException();
    }

    public async Task<(IEnumerable<Lot> lots, int TotalPages)> GetAllFilteredAsync(
        Expression<Func<Lot, bool>> predicate, FilteringOptionsDto filterOptions)
    {
        var lots = _context.Lots
            .Include(lot => lot.Bets)
            .Where(predicate);

        if (filterOptions.PriceOrder != null)
        {
            lots = await lots.GetSorted(filterOptions);
        }

        var pagedLots = lots.GetPaged(filterOptions);
        return (pagedLots.Result.Items, pagedLots.Result.TotalPages);
    }
}