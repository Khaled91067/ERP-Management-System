
namespace ERP.Application.Abstractions.Messaging;

using global::ERP.Domain.Shared.Base;
using global::ERP.Domain.Shared.Abstractions;

using MediatR;

public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent) :
    INotification where TDomainEvent : IDomainEvent;
