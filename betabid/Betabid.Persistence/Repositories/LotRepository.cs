using Betabid.Application.Interfaces.Repositories;
using Betabid.Domain.Entities;
using Betabid.Persistence.Context;
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
            .FirstOrDefaultAsync(lot => lot.Id == id);
    }
}