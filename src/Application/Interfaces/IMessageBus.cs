using Application.Dto;

namespace Application.Interfaces;

public interface IMessageBus
{
    Task PublishToUser(ConversationStreamingMessage payload);
}