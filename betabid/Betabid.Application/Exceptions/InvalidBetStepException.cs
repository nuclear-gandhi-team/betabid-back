namespace Betabid.Application.Exceptions;

public class InvalidBetStepException : Exception
{
    public InvalidBetStepException()
    {
    }

    public InvalidBetStepException(string message)
        : base(message)
    {
    }

    public InvalidBetStepException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}