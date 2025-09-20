using Application.Interfaces;
using Domain.Conversations;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Infrastructure.Assistants;

public class Assistant(ChatCompletionAgent chatCompletionAgent) : IAssistant
{
    public async IAsyncEnumerable<string> StreamAsync(UserMessage userMessage)
    {
        await foreach (var response in chatCompletionAgent.InvokeAsync(new ChatMessageContent(AuthorRole.User, userMessage.Content)))
        {
            if (response.Message .Content != null)
            {
                yield return response.Message.Content;
            }
        }
    }
}