using Application.Interfaces;
using Domain.Conversations;
using Infrastructure.Interfaces;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Infrastructure.Assistants;

public class DefaultMemoryStrategy : IAssistantMemory
{
    public ChatHistoryAgentThread Initialize(Conversation conversation)
    {
      
        var agentThread = new ChatHistoryAgentThread();

        foreach (var conversationThreadMessage in conversation.Threads.SelectMany(conversationThread => conversationThread.Messages))
        {
            switch (conversationThreadMessage.Role)
            {
                case "user":
                    agentThread.ChatHistory.Add(new ChatMessageContent(AuthorRole.User, conversationThreadMessage.Content));
                    break;
                case "assistant":
                    agentThread.ChatHistory.Add(new ChatMessageContent(AuthorRole.Assistant, conversationThreadMessage.Content));
                    break;
                default:
                    throw new InvalidOperationException($"Unexpected role '{conversationThreadMessage.Role}' in conversation thread message.");
            }
        }
        

        return agentThread;
    }
}