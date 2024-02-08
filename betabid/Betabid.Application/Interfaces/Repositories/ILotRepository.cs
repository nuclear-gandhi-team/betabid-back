using Betabid.Domain.Entities;

namespace Betabid.Application.Interfaces.Repositories;

public interface ILotRepository : IRepository<Lot>
{
    Task<bool> IsLotSavedByUserAsync(int lotId, string userId);

    Task<Lot> GetByIdWithTagsAsync(int id);
}