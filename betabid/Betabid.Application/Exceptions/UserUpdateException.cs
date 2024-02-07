namespace Betabid.Application.Exceptions;

public class UserUpdateException : Exception
{
    public UserUpdateException()
    {
    }

    public UserUpdateException(string message)
        : base(message)
    {
    }

    public UserUpdateException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
