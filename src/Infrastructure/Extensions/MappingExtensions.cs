using Domain;
using Domain.Conversations;
using Infrastructure.Dto;

namespace Infrastructure.Extensions;

public static class MappingExtensions
{
    private static ConversationMessageDto Map(this Message message)
    {
        return new ConversationMessageDto(message.Id, message.Role, message.Content);
    }

    private static ConversationThreadDto Map(this Domain.Conversations.ConversationThread thread)
    {
        var conversationStorageMessages = thread.Messages.Select(message => message.Map()).ToList();

        return new ConversationThreadDto(thread.Id, conversationStorageMessages);
    }

    public static ConversationDto Map(this Domain.Conversations.Conversation conversation)
    {
        var conversationStorageThreads = conversation.Threads.Select(thread => thread.Map()).ToList();

        return new ConversationDto(conversation.Id, conversation.UserId, conversation.Name, conversation.CurrentThread,
            conversationStorageThreads);
    }
    private static Message Map(this ConversationMessageDto messageDto)
    {
        return messageDto.Role switch
        {
            "assistant" => new AssistantMessage(messageDto.Id, messageDto.Content),
            "user" => new UserMessage(messageDto.Id, messageDto.Content),
            _ => throw new InvalidOperationException($"{messageDto.Role} is not a valid Message Type.")
        };
    }

    private static Domain.Conversations.ConversationThread Map(this ConversationThreadDto threadDto)
    {
        var messages = threadDto.Messages.Select(storageMessage => storageMessage.Map()).ToList();

        return new Domain.Conversations.ConversationThread(threadDto.Id, messages);
    }

    public static Domain.Conversations.Conversation Map(this ConversationDto conversationDto)
    {
        var threads = conversationDto.Threads.Select(storageThread => storageThread.Map()).ToList();

        return new Domain.Conversations.Conversation(conversationDto.Id,conversationDto.UserId, conversationDto.Name, conversationDto.CurrentThread, threads);
    }
}