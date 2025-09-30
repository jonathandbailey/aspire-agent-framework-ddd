using Application.Events.Integration;
using Application.Interfaces;
using MediatR;

namespace Application.Services;

public class StreamingEventPublisher(IMediator mediator) : IStreamingEventPublisher
{
    public async Task Send(UserStreamingApplicationEvent userStreamingApplicationEvent)
    {
        await mediator.Send(userStreamingApplicationEvent);
    }
}