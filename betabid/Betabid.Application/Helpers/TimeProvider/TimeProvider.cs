namespace Betabid.Application.Helpers;

public class TimeProvider : ITimeProvider
{
    public DateTime Now => DateTime.Now;
}