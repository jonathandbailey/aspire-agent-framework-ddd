using Agents.Infrastructure.Dto;
using Azure.Core;
using Microsoft.Extensions.AI;

namespace Agents.Conversation.Extensions;

public static class MessageExtensions
{
    public static List<ChatMessage> Map(this List<Message> source)
    {
        var messages = new List<ChatMessage>();

        foreach (var message in source)
        {
            if (message.Role == "user")
            {
                var userChatMessage = new ChatMessage(ChatRole.User, message.Content);

                messages.Add(userChatMessage);
            }

            if (message.Role == "assistant")
            {
                var userChatMessage = new ChatMessage(ChatRole.Assistant, message.Content);

                messages.Add(userChatMessage);
            }
        }

        return messages;
    }
}