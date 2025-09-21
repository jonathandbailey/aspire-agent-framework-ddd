namespace Infrastructure.Exceptions;

public class ConversationInfrastructureException : Exception
{
    public ConversationInfrastructureException()
    {
    }

    public ConversationInfrastructureException(string message) : base(message)
    {
    }

    public ConversationInfrastructureException(string message, Exception inner) : base(message, inner)
    {
    }
}