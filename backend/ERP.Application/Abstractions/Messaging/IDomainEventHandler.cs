
namespace ERP.Application.Abstractions.Messaging;

using ERP.Domain.Shared.Base;
using ERP.Domain.Shared.Abstractions;

using MediatR;

public interface IDomainEventHandler<TDomainEvent>: INotificationHandler<DomainEventNotification<TDomainEvent>>
    where TDomainEvent : IDomainEvent
{
}