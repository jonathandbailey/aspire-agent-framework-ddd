using Application.Dto;
using Application.Interfaces;
using Domain.Conversations;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text;
using Infrastructure.Dto;
using Infrastructure.Parsers;

namespace Infrastructure.Assistants;

public class TitleAssistant(ChatCompletionAgent chatCompletionAgent) : ITitleAssistant
{
    public async Task<AssistantResponseDto> InvokeAsync(string content)
    {
        var stringBuilder = new StringBuilder();

        await foreach (var response in chatCompletionAgent.InvokeAsync(new ChatMessageContent(AuthorRole.User, content)))
        {
            if (response.Message .Content != null)
            {
                stringBuilder.Append(response.Message.Content);
            }
        }

        var fullResponse = stringBuilder.ToString();

        if (!JsonOutputParser.HasJson(fullResponse))
            throw new InvalidOperationException($"Title Assistant response does not contain valid JSON {fullResponse}");
        

        var conversationTitle = JsonOutputParser.Parse<AssistantResponseJsonTitle>(fullResponse);

        return new AssistantResponseDto { Content = conversationTitle.Title };
    }
}