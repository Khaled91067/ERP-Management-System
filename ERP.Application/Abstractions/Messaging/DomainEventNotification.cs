using ERP.Domain.Common;
using MediatR;

namespace ERP.Application.Abstractions.Messaging;

public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent) :
    INotification where TDomainEvent : IDomainEvent;