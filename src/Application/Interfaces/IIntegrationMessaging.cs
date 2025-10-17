namespace Application.Interfaces;

public interface IIntegrationMessaging
{
    Task SendAsync<T>(T payload, string target);
    Task SendAsync<T>(T payload);
    Task SendAsyncToSummarize<T>(T payload);
}