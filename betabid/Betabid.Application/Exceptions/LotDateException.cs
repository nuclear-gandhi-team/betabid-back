namespace Betabid.Application.Exceptions;

public class LotDateException : Exception
{
    public LotDateException()
    {
    }

    public LotDateException(string message)
        : base(message)
    {
    }

    public LotDateException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}