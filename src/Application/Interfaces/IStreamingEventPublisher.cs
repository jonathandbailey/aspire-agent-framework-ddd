using Application.Events.Integration;

namespace Application.Interfaces;

public interface IStreamingEventPublisher
{
    Task Send(UserStreamingApplicationEvent userStreamingApplicationEvent);
}