
namespace ERP.Application.Abstractions.Messaging;

using global::ERP.Domain.Shared.Base;
using global::ERP.Domain.Shared.Abstractions;

using MediatR;

public interface IDomainEventHandler<TDomainEvent>: INotificationHandler<DomainEventNotification<TDomainEvent>>
    where TDomainEvent : IDomainEvent
{
}
