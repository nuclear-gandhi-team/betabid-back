using Betabid.Domain.Entities;
using Betabid.Domain.Enums;

namespace Betabid.Application.Helpers;

public class StatusHelper
{
    private readonly ITimeProvider _timeProvider;
    
    public StatusHelper(ITimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }
    
    public string GetStatus(Lot lot)
    {
        if (_timeProvider.Now < lot.DateStarted)
        {
            return LotStatus.Preparing.ToString();
        }
        if (_timeProvider.Now >= lot.DateStarted && _timeProvider.Now < lot.Deadline)
        {
            return LotStatus.Open.ToString();
        }
        return LotStatus.Finished.ToString();
    } 
}