using ERP.Application.Abstractions.Messaging;
using ERP.Domain.Common;
using MediatR;

namespace ERP.Application.Messaging;

public sealed class DomainEventDispatcher(IPublisher publisher) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            var notificationType = typeof(DomainEventNotification<>)
                .MakeGenericType(domainEvent.GetType());

            var notification = Activator.CreateInstance(notificationType, domainEvent)!;

            await publisher.Publish(notification, cancellationToken);
        }
    }
}