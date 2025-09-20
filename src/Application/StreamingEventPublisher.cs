using Application.Events;
using Application.Interfaces;
using MediatR;

namespace Application;

public class StreamingEventPublisher(IMediator mediator) : IStreamingEventPublisher
{
    public async Task Send(StreamingApplicationEvent streamingApplicationEvent)
    {
        await mediator.Send(streamingApplicationEvent);
    }
}