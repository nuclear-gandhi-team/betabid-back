using System.Linq.Expressions;
using Betabid.Domain.Entities;

namespace Betabid.Application.Filtering.Lots;

public class TagsFilteringCriteria : IFilteringCriteria<Lot>
{
    private readonly IList<string> _tags;

    public TagsFilteringCriteria(IList<string> tags)
    {
        _tags = tags;
    }
    
    public Expression<Func<Lot, bool>> Criteria => lot => 
        lot.Tags.Any(lotTag => _tags.Any(tag => tag == lotTag.Name));
}