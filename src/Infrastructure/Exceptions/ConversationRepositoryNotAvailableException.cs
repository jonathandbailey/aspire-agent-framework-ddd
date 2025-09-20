namespace Infrastructure.Exceptions;

public class ConversationRepositoryNotAvailableException : Exception
{
    public ConversationRepositoryNotAvailableException()
    {
    }

    public ConversationRepositoryNotAvailableException(string message) : base(message)
    {
    }

    public ConversationRepositoryNotAvailableException(string message, Exception inner) : base(message, inner)
    {
    }
}