namespace Betabid.Application.Exceptions;

public class EmptyCommentException : Exception
{
    public EmptyCommentException()
    {
    }

    public EmptyCommentException(string message)
        : base(message)
    {
    }

    public EmptyCommentException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}