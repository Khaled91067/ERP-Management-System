
namespace ERP.Application.Abstractions.Messaging;

using ERP.Domain.Shared.Common;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(
        IEnumerable<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default);
}