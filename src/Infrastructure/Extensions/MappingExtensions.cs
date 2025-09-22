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


    private static ConversationTurnDto Map(this ConversationTurn turn)
    {
        return new ConversationTurnDto(turn.Id, turn.Index, turn.UserMessage.Map(), turn.AssistantMessage.Map());
    }

    private static ConversationTurn Map(this ConversationTurnDto turnDto)
    {
        return new ConversationTurn(turnDto.Id, turnDto.Index, turnDto.UserMessage.MapUserMessage(), turnDto.AssistantMessage.MapAssistantMessage());
    }

    private static ConversationThreadDto Map(this ConversationThread thread)
    {
        var turns = thread.Turns.Select(turn => turn.Map()).ToList();

        return new ConversationThreadDto(thread.Id, thread.Index, turns);
    }

    public static ConversationDto Map(this Conversation conversation)
    {
        var conversationStorageThreads = conversation.Threads.Select(thread => thread.Map()).ToList();

        return new ConversationDto(conversation.Id, conversation.UserId, conversation.Name, conversation.CurrentThread,
            conversationStorageThreads);
    }
   
    private static AssistantMessage MapAssistantMessage(this ConversationMessageDto messageDto)
    {
        return messageDto.Role switch
        {

            "assistant" => new AssistantMessage(messageDto.Id, messageDto.Index, messageDto.Content),
            _ => throw new InvalidOperationException($"{messageDto.Role} is not a valid Message Type.")
        };
    }


    private static UserMessage MapUserMessage(this ConversationMessageDto messageDto)
    {
        return messageDto.Role switch
        {
           
            "user" => new UserMessage(messageDto.Id, messageDto.Index, messageDto.Content),
            _ => throw new InvalidOperationException($"{messageDto.Role} is not a valid Message Type.")
        };
    }

    private static ConversationThread Map(this ConversationThreadDto threadDto)
    {
        var turns = threadDto.Turns.Select(turn => turn.Map()).ToList();

        return new ConversationThread(threadDto.Id, threadDto.Index, turns);
    }

    public static Conversation Map(this ConversationDto conversationDto)
    {
        var threads = conversationDto.Threads.Select(storageThread => storageThread.Map()).ToList();

        return new Conversation(conversationDto.Id,conversationDto.UserId, conversationDto.Name, conversationDto.CurrentThread, threads);
    }
}