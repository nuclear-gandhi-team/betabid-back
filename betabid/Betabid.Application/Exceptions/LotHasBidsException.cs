namespace Betabid.Application.Exceptions;

public class LotHasBidsException : Exception
{
    public LotHasBidsException()
    {
    }
    
    public LotHasBidsException(string message)
        : base(message)
    {
    }
    
    public LotHasBidsException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
    
}