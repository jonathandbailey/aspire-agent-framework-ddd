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
    private const string ApiConversationPath = "api/conversations";
    private const string ApiConversationSummariesPath = "api/conversations/summaries";
    private const string ApiChatPath = "api/chat";
    private const string ApiConversationById = "api/conversations/{conversationId:guid}";

    public static WebApplication MapChatApi(this WebApplication app)
    {
        app.MapPost(ApiChatPath, async ([FromBody] ChatRequestDto requestDto, IMediator mediator, HttpContext context) =>
        {
            await mediator.Send(requestDto.Map(context.User.Id()));
 
            return Results.Ok();
        });
        
        return app;
    }

    public static WebApplication MapConversationApi(this WebApplication app)
    {
        app.MapGet(ApiConversationPath, 
            async (IConversationQuery conversationQuery, HttpContext context) => Results.Ok(await conversationQuery.GetAllConversationsAsync(context.User.Id())));

        app.MapGet(ApiConversationSummariesPath,
            async (IConversationQuery conversationQuery, HttpContext context) => Results.Ok(await conversationQuery.GetConversationSummaries(context.User.Id())));


        app.MapGet(ApiConversationById, 
            async (Guid conversationId, IConversationQuery conversationQuery, HttpContext context) => Results.Ok(await conversationQuery.LoadAsync(context.User.Id(), conversationId)));

        app.MapPost(ApiConversationPath, 
            async (IMediator mediator, HttpContext context) => Results.Ok(await mediator.Send(new CreateConversationCommand(context.User.Id()))));

        return app;
    }
}