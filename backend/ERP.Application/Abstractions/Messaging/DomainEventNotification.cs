
namespace ERP.Application.Abstractions.Messaging;

using ERP.Domain.Shared.Abstractions;

using MediatR;

public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent) :
    INotification where TDomainEvent : IDomainEvent;
