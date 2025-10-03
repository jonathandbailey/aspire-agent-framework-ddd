namespace Application.Interfaces;

public interface IMessageBus
{
    Task SendAsync<T>(T payload, string target);
    Task SendAsync<T>(T payload);
    Task SendAsyncToSummarize<T>(T payload);
}