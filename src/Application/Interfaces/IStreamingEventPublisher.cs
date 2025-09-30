using Application.Events.Integration;

namespace Application.Interfaces;

public interface IStreamingEventPublisher
{
    Task Send<T>(T applicationEvent);
}