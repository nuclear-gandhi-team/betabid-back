using System.Linq.Expressions;
using Betabid.Domain.Entities;

namespace Betabid.Application.Filtering.Lots;

public class LotOwnerFilteringCriteria  : IFilteringCriteria<Lot>
{
    private readonly string _userId;

    public LotOwnerFilteringCriteria(string userId)
    {
        _userId = userId;
    }
    
    public Expression<Func<Lot, bool>> Criteria => lot => lot.OwnerId == _userId;
}