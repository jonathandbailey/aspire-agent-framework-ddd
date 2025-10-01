namespace Application.Interfaces;

public interface IMessageBus
{
    Task SendAsync<T>(T payload, string target);
    Task SendAsync<T>(T payload);
}