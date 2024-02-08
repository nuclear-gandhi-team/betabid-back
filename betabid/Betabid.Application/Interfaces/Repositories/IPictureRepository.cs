using Betabid.Domain.Entities;

namespace Betabid.Application.Interfaces.Repositories;

public interface IPictureRepository : IRepository<Picture>
{
    public Task<List<Picture>> GetPicturesByLotIdAsync(int lotId);
    
    public Task<Picture?> GetPictureByLotIdAsync(int lotId);
}