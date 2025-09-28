using Application.Dto;
using Application.Interfaces;
using Domain.Conversations;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Infrastructure.Agents;

public class ConversationAgent(IStreamingAgent agent) : IConversationAgent
{
    public async IAsyncEnumerable<AssistantResponseDto> InvokeStreamAsync(
        List<Message> messages)
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
        
        await foreach (var response in agent.InvokeStreamAsync(thread))
        {
            yield return new AssistantResponseDto { Content = response.Message.Content!};
        }
    }
}