using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Services;

public class StreamingEventPublisher(IMediator mediator) : IStreamingEventPublisher
{
    public async Task Send<T>(T applicationEvent)
    {
        Verify.NotNull(applicationEvent);
        
        await mediator.Send(applicationEvent);
    }
}