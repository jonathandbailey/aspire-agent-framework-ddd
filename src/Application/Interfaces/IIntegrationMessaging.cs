namespace Application.Interfaces;

public interface IIntegrationMessaging
{
    Task SendAgentMessageAsync<T>(T payload);
}