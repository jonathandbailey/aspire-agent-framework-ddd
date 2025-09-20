using Application.Events;

namespace Application.Interfaces;

public interface IStreamingEventPublisher
{
    Task Send(StreamingApplicationEvent streamingApplicationEvent);
}