namespace Betabid.Application.Helpers;

public interface ITimeProvider
{
    DateTime Now { get; }
}