using Api.Extensions;
using Application.Conversations.Commands;
using Application.Conversations.Queries;
using Application.Dto;
using Application.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api;

public static class ApiMappings
{
    private const string ApiConversationsRoot = "api/conversations";
    private const string ApiConversationExchange = "/{conversationId:guid}/exchanges";
    private const string ApiConversationsPath = "/";
    private const string ApiConversationSummariesPath = "summaries";
    private const string ApiChatPath = "api/chat";
    private const string ApiConversationById = "{conversationId:guid}";

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
        var api = app.MapGroup(ApiConversationsRoot);

        api.MapGet(ApiConversationsPath, 
            async (IMediator mediator, HttpContext context) 
                => Results.Ok(await mediator.Send(new GetConversationsQuery(context.User.Id()))));

        api.MapGet(ApiConversationSummariesPath,
            async (IMediator mediator, HttpContext context) => Results.Ok(await mediator.Send(new GetConversationSummariesQuery(context.User.Id()))));


        api.MapGet(ApiConversationById, 
            async (Guid conversationId, IMediator mediator, HttpContext context) => Results.Ok(await mediator.Send(new GetConversationByIdQuery(context.User.Id(), conversationId))));

        api.MapPost(ApiConversationsPath, 
            async (IMediator mediator, HttpContext context) => Results.Ok(await mediator.Send(new CreateConversationCommand(context.User.Id()))));

        api.MapPost(ApiConversationExchange,
            async (Guid conversationId, IMediator mediator, HttpContext context) => Results.Ok(await mediator.Send(new CreateConversationExchangeCommand(context.User.Id(), conversationId))));


        return app;
    }
}