using System.Linq.Expressions;
using Betabid.Application.Helpers;
using Betabid.Domain.Entities;
using Betabid.Domain.Enums;

namespace Betabid.Application.Filtering.Lots;

public class LotStatusFilteringCriteria : IFilteringCriteria<Lot>
{
    private readonly LotStatus _status;
    private readonly ITimeProvider _timeProvider;

    public LotStatusFilteringCriteria(LotStatus status, ITimeProvider timeProvider)
    {
        _status = status;
        _timeProvider = timeProvider;
    }
    
    public Expression<Func<Lot, bool>> Criteria => _status switch
    {
        LotStatus.Open => lot => 
            lot.DateStarted <= _timeProvider.Now && lot.Deadline >= _timeProvider.Now,
        LotStatus.Preparing => lot =>
            lot.DateStarted > _timeProvider.Now,
        _ => throw new ArgumentException($"Invalid status: {_status}")
    };
}