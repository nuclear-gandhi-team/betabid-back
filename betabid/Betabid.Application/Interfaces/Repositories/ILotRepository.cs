using System.Linq.Expressions;
using Betabid.Application.DTOs.FilteringDto;
using Betabid.Domain.Entities;

namespace Betabid.Application.Interfaces.Repositories;

public interface ILotRepository : IRepository<Lot>
{
    Task<bool> IsLotSavedByUserAsync(int lotId, string userId);

    Task<Lot> GetByIdWithTagsAsync(int id);

    Task<Lot> GetByIdWithBetsAsync(int id);

    Task<Lot> GetByIdWithCommentsAsync(int id);
    
    Task<(IList<Lot> lots, int TotalPages)> GetAllFilteredAsync(
        Expression<Func<Lot, bool>> predicate, FilteringOptionsDto filterOptions);
}