namespace Betabid.Application.Exceptions;

public class LotAlreadySavedException : Exception
{
    public LotAlreadySavedException() : base("The lot is already saved by the user.")
    {
    }
    
    public LotAlreadySavedException(string message)
        : base(message)
    {
    }
    
    public LotAlreadySavedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
    
}