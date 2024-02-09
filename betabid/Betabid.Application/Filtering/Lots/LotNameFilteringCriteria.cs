using System.Linq.Expressions;
using Betabid.Domain.Entities;

namespace Betabid.Application.Filtering.Lots;

public class LotNameFilteringCriteria : IFilteringCriteria<Lot>
{
    private readonly string _name;

    public LotNameFilteringCriteria(string name)
    {
        _name = name;
    }
    
    public Expression<Func<Lot, bool>> Criteria => lot => lot.Name.StartsWith(_name);
}