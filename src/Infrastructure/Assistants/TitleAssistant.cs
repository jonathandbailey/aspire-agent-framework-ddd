using Application;
using Application.Dto;
using Application.Interfaces;
using Domain.Conversations;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text;
using Infrastructure.Dto;

namespace Infrastructure.Assistants;

public class TitleAssistant(ChatCompletionAgent chatCompletionAgent) : ITitleAssistant
{
    public async Task<AssistantResponseDto> InvokeAsync(UserMessage userMessage)
    {
        var stringBuilder = new StringBuilder();

        await foreach (var response in chatCompletionAgent.InvokeAsync(new ChatMessageContent(AuthorRole.User, userMessage.Content)))
        {
            if (response.Message .Content != null)
            {
                stringBuilder.Append(response.Message.Content);
            }
        }

        var content = stringBuilder.ToString();

        if (!JsonOutputParser.HasJson(content))
            throw new InvalidOperationException($"Title Assistant response does not contain valid JSON {content}");
        

        var conversationTitle = JsonOutputParser.Parse<AssistantResponseJsonTitle>(content);

        return new AssistantResponseDto { Content = conversationTitle.Title };
    }
}