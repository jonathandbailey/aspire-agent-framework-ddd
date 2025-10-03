using Agents.Infrastructure.Dto;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using AgentThread = Microsoft.Agents.AI.AgentThread;

namespace Agents.Infrastructure.Extensions;

public static class AgentExtensions
{
    public static AgentThread ToAgentThread(this AgentThread thread, List<Message> messages)
    {
        var store = thread.GetService<ChatMessageStore>();

        if (store == null)
            throw new Exception("The current agent thread does not have a Chat Message Store registered as a service.");

        foreach (var message in messages)
        {
            if (message.Role == "user")
            {
                var userChatMessage = new ChatMessage(ChatRole.User, message.Content);

                store.AddMessagesAsync([userChatMessage]);
            }

            if (message.Role == "assistant")
            {
                var userChatMessage = new ChatMessage(ChatRole.Assistant, message.Content);

                store.AddMessagesAsync([userChatMessage]);
            }
        }

        return thread;
    }
}