namespace Application.Interfaces;

public interface IConversationClient
{
    Task ChatWithUser<T>(Guid userId, T payload);

    Task ExecuteCommand<T>(Guid userId, T payload);
}