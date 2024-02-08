using System.Linq.Expressions;

namespace Betabid.Application.Filtering;

public interface IFilteringCriteria<T>
{
    Expression<Func<T, bool>> Criteria { get; }
}