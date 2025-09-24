using Domain.Common;
using MediatR;

namespace Application.Extensions;

public static class MediatRExtensions
{
    public static async Task PublishDomainEvents(this IMediator mediator, Entity entity)
    {
        foreach (var notification in entity.DomainEvents)
        {
            await mediator.Publish(notification);
        }

        entity.ClearDomainEvents();
    }
}