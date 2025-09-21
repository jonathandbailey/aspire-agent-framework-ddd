using Domain;
using Domain.Conversations;
using Infrastructure.Dto;

namespace Infrastructure.Extensions;

public static class MappingExtensions
{
    private static ConversationMessageDto Map(this Message message)
    {
        return new ConversationMessageDto(message.Id, message.Index, message.Role, message.Content);
    }

    private static ConversationThreadDto Map(this ConversationThread thread)
    {
        var conversationStorageMessages = thread.Messages.Select(message => message.Map()).ToList();

        return new ConversationThreadDto(thread.Id, thread.Index, conversationStorageMessages);
    }

    public static ConversationDto Map(this Conversation conversation)
    {
        var conversationStorageThreads = conversation.Threads.Select(thread => thread.Map()).ToList();

        return new ConversationDto(conversation.Id, conversation.UserId, conversation.Name, conversation.CurrentThread,
            conversationStorageThreads);
    }
    private static Message Map(this ConversationMessageDto messageDto)
    {
        return messageDto.Role switch
        {
            "assistant" => new AssistantMessage(messageDto.Id, messageDto.Index,  messageDto.Content),
            "user" => new UserMessage(messageDto.Id, messageDto.Index, messageDto.Content),
            _ => throw new InvalidOperationException($"{messageDto.Role} is not a valid Message Type.")
        };
    }

    private static ConversationThread Map(this ConversationThreadDto threadDto)
    {
        var messages = threadDto.Messages.Select(storageMessage => storageMessage.Map()).ToList();

        return new ConversationThread(threadDto.Id, threadDto.Index, messages);
    }

    public static Conversation Map(this ConversationDto conversationDto)
    {
        var threads = conversationDto.Threads.Select(storageThread => storageThread.Map()).ToList();

        return new Conversation(conversationDto.Id,conversationDto.UserId, conversationDto.Name, conversationDto.CurrentThread, threads);
    }
}