namespace Application.Interfaces;

public interface IMessageBus
{
    Task SendAsync<T>(T payload, string target);
}