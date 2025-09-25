using Api.Extensions;
using Application.Conversations.Commands;
using Application.Conversations.Queries;
using Application.Dto;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api;

public static class ApiMappings
{
    private const string ApiConversationsRoot = "api/conversations";
    private const string CreateConversationPattern = "/";

    private const string GetConversationByIdPattern = "{conversationId:guid}";
    private const string GetConversationSummaries = "summaries";

    private const string CreateConversationExchangePattern = "/{conversationId:guid}/exchanges";
    private const string StartConversationExchangePattern = "/{conversationId:guid}/exchanges/{exchangeId:guid}/messages";


    public static WebApplication MapConversationApi(this WebApplication app)
    {
        var api = app.MapGroup(ApiConversationsRoot);

        api.MapPost(StartConversationExchangePattern, ConversationExchange);

        api.MapGet(GetConversationSummaries, ConversationSummaries);

        api.MapGet(GetConversationByIdPattern, GetConversationById);

        api.MapPost(CreateConversationPattern, CreateConversation);
        api.MapPost(CreateConversationExchangePattern,CreateConversationExchange);

        return app;
    }

    private static async Task<Created<Guid>> CreateConversation(IMediator mediator, HttpContext context)
    {
        var conversationId = await mediator.Send(new CreateConversationCommand(context.User.Id()));

        return TypedResults.Created($"api/conversations/{conversationId}", conversationId);
    }

    private static async Task<Ok> ConversationExchange(Guid conversationId, Guid exchangeId,
        [FromBody] ChatRequestDto requestDto, IMediator mediator, HttpContext context)
    {
        await mediator.Send(
            new StartConversationExchangeCommand(requestDto.Message, context.User.Id(), conversationId, exchangeId));

        return TypedResults.Ok();
    }
    private static async Task<Ok<List<ConversationSummaryItem>>> ConversationSummaries(IMediator mediator, HttpContext context)
    {
        return TypedResults.Ok(await mediator.Send(new GetConversationSummariesQuery(context.User.Id())));
    }

    private static async Task<Created<Guid>> CreateConversationExchange(Guid conversationId, IMediator mediator,
        HttpContext context)
    {
        var exchangeId =  await mediator.Send(new CreateConversationExchangeCommand(context.User.Id(), conversationId));

        return TypedResults.Created($"api/conversations//{conversationId}/exchanges/{exchangeId}", exchangeId);
    }

    private static async Task<Ok<Conversation>> GetConversationById(Guid conversationId, IMediator mediator,
        HttpContext context)
    {
        return TypedResults.Ok(await mediator.Send(new GetConversationByIdQuery(context.User.Id(), conversationId)));
    }
}