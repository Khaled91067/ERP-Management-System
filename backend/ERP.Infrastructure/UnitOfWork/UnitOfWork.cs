using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Messaging;
using ERP.Domain.Common;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.UnitOfWork
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        public UnitOfWork(AppDbContext context,IDomainEventDispatcher domainEventDispatcher)
        {
            _context = context;
            _domainEventDispatcher = domainEventDispatcher;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var aggregates = _context.ChangeTracker
                .Entries<AggregateRoot>()
                .Where(entry => entry.Entity.DomainEvents.Any())
                .Select(entry => entry.Entity)
                .ToList();

            var domainEvents = aggregates
                .SelectMany(aggregate => aggregate.DomainEvents)
                .ToList();

            await _domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);

            var result = await _context.SaveChangesAsync(cancellationToken);

            foreach (var aggregate in aggregates)
            {
                aggregate.ClearDomainEvents();
            }

            return result;
        }
    }
}
