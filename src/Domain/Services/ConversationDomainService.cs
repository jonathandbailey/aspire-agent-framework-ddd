using System.Text;
using Domain.Conversations;
using Domain.Interfaces;

namespace Domain.Services;

public class ConversationDomainService : IConversationDomainService
{
    public string GetConversationSummary(Conversation conversation)
    {
        if (conversation.Threads.Count == 0)
        {
            throw new InvalidOperationException($"Conversation {conversation.Id} does not contain any threads.");
        }
        
        var thread = conversation.Threads.First();

        if (thread.Exchanges.Count == 0)
        {
            throw new InvalidOperationException($"Conversation Thread {thread.Id}, doesn't have any conversations turns. Conversation Id : {conversation.Id}");
        }

        var stringBuilder = new StringBuilder();

        foreach (var turn in thread.Exchanges)
        {
            stringBuilder.AppendLine(turn.UserMessage.Content);
            stringBuilder.AppendLine(turn.AssistantMessage.Content);
        }

        return stringBuilder.ToString();
    }

    public List<Message> GetMessages(Conversation conversation)
    {
        var messages = new List<Message>();

        var index = 0;

        foreach (var thread in conversation.Threads)
        {
            foreach (var exchange in thread.Exchanges)
            {
                messages.Add(new Message(exchange.UserMessage.Id, index++, exchange.UserMessage.Content, exchange.UserMessage.Role));
                messages.Add(new Message(exchange.AssistantMessage.Id, index++, exchange.AssistantMessage.Content, exchange.AssistantMessage.Role));
            }
        }
        
        return messages;
    }

}