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

        foreach (var thread in conversation.Threads)
        {
            foreach (var turn in thread.Turns)
            {
                agentThread.ChatHistory.Add(new ChatMessageContent(AuthorRole.User, turn.UserMessage.Content));
                agentThread.ChatHistory.Add(new ChatMessageContent(AuthorRole.Assistant, turn.AssistantMessage.Content));
            }
        }

        return agentThread;
    }
}