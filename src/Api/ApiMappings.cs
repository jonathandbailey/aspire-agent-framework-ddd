using Api.Extensions;
using Application.Commands;
using Application.Dto;
using Application.Extensions;
using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api;

public static class ApiMappings
{
    private const string ApiConversationPath = "api/conversation";
    private const string ApiChatPath = "api/chat";
    private const string ApiConversationById = "api/conversation/{conversationId:guid}";

    public static WebApplication MapChatApi(this WebApplication app)
    {
        app.MapPost(ApiChatPath, async ([FromBody] ChatRequestDto requestDto, IMediator mediator, HttpContext context) =>
        {
            var message = await mediator.Send(requestDto.Map(context.User.Id()));
 
            return Results.Ok(message.MapToChatResponseDto(requestDto.ConversationId));
        });
        
        return app;
    }

    public static WebApplication MapConversationApi(this WebApplication app)
    {
        app.MapGet(ApiConversationPath, async (IConversationQuery conversationQuery) => Results.Ok(await conversationQuery.GetAllConversationsAsync()));

        app.MapGet(ApiConversationById, 
            async (Guid conversationId, IConversationQuery conversationQuery) => Results.Ok(await conversationQuery.LoadAsync(conversationId)));

        app.MapPost(ApiConversationPath, 
            async (IMediator mediator, HttpContext context) => Results.Ok(await mediator.Send(new CreateConversationCommand(context.User.Id()))));

        return app;
    }
}