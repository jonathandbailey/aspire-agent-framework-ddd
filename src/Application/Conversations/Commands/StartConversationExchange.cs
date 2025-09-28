using System.Text;
using Application.Events.Integration;
using Application.Interfaces;
using Domain.Conversations;
using Domain.Interfaces;
using MediatR;

namespace Application.Conversations.Commands;

public class StartConversationExchangeCommandHandler(IConversationRepository conversationRepository,
    IAssistantFactory assistantFactory, 
    IConversationDomainService conversationDomainService,
        IStreamingEventPublisher publisher
    ) : IRequestHandler<StartConversationExchangeCommand>
{
    public async Task Handle(StartConversationExchangeCommand request, CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.LoadAsync(request.UserId, request.ConversationId);
            
        conversation.StartConversationExchange(request.Message, ExchangeId.FromGuid(request.ExchangeId));

        await conversationRepository.SaveAsync(conversation);

        var assistant = await assistantFactory.CreateConversationAgent();

        var messages = conversationDomainService.GetMessages(conversation);

        var stringBuilder = new StringBuilder();

        await foreach (var response in assistant.InvokeStreamAsync(messages))
        {
            await publisher.Send(new StreamingApplicationEvent(conversation.UserId, request.ExchangeId, conversation.Id, response.Content));

            stringBuilder.Append(response.Content);
        }
    
        conversation.CompleteConversationExchange(ExchangeId.FromGuid(request.ExchangeId), stringBuilder.ToString());

        await conversationRepository.SaveAsync(conversation);
    }
}

public sealed record StartConversationExchangeCommand(string Message, Guid UserId, Guid ConversationId, Guid ExchangeId) : IRequest;
