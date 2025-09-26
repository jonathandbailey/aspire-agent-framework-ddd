namespace Domain.Exceptions;

public class ExchangeNotFoundDomainException : Exception
{
    public ExchangeNotFoundDomainException()
    {
    }

    public ExchangeNotFoundDomainException(string message) : base(message)
    {
    }

    public ExchangeNotFoundDomainException(string message, Exception inner) : base(message, inner)
    {
    }
}