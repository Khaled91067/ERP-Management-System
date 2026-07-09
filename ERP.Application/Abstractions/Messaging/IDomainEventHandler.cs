using ERP.Domain.Common;
using MediatR;

namespace ERP.Application.Abstractions.Messaging;

public interface IDomainEventHandler<TDomainEvent>: INotificationHandler<DomainEventNotification<TDomainEvent>>
    where TDomainEvent : IDomainEvent
{
}