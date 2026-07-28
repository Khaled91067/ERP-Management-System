
namespace ERP.Application.Abstractions.Messaging;

using ERP.Domain.Shared.Common;

using MediatR;

public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent) :
    INotification where TDomainEvent : IDomainEvent;