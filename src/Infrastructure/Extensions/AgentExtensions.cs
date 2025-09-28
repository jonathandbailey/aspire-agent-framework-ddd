using Domain.Conversations;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Infrastructure.Extensions;

public static class AgentExtensions
{
    public static ChatHistoryAgentThread ToChatHistoryThread(this List<Message> messages)
    {
        var thread = new ChatHistoryAgentThread();

        foreach (var message in messages)
        {
            if (message.Role == "user")
            {
                thread.ChatHistory.Add(new ChatMessageContent(AuthorRole.User, message.Content));
            }

            if (message.Role == "assistant")
            {
                thread.ChatHistory.Add(new ChatMessageContent(AuthorRole.Assistant, message.Content));
            }
        }

        return thread;
    }
}