namespace Betabid.Application.Exceptions;

public class LotAlreadyStartedException : Exception
{
    public LotAlreadyStartedException()
    {
    }
    
    public LotAlreadyStartedException(string message)
        : base(message)
    {
    }

    public LotAlreadyStartedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}