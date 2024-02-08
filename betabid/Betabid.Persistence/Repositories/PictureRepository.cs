using Betabid.Application.Interfaces.Repositories;
using Betabid.Domain.Entities;
using Betabid.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Betabid.Persistence.Repositories;

public class PictureRepository : Repository<Picture>, IPictureRepository
{
    private readonly DataContext _context;
    public PictureRepository(DataContext dbContext, DataContext context) : base(dbContext)
    {
        _context = context;
    }

    public async Task<List<Picture>> GetPicturesByLotIdAsync(int lotId)
    {
        return await _context.Pictures.Where(p => p.LotId == lotId).ToListAsync();
    }

    public Task<Picture?> GetPictureByLotIdAsync(int lotId)
    {
        return _context.Pictures.FirstOrDefaultAsync(p => p.LotId == lotId);
    }
}