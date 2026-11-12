namespace ERP.Infrastructure.UnitOfWork
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using ERP.Application.Abstractions;
    using ERP.Application.Abstractions.Messaging;
    using ERP.Domain.Shared.Base;
    using ERP.Infrastructure.Persistence;

    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        public UnitOfWork(AppDbContext context, IDomainEventDispatcher domainEventDispatcher)
        {
            _context = context;
            _domainEventDispatcher = domainEventDispatcher;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            while (true)
            {
                var aggregates = _context.ChangeTracker
                    .Entries<AggregateRoot>()
                    .Where(entry => entry.Entity.DomainEvents.Any())
                    .Select(entry => entry.Entity)
                    .ToList();

                if (aggregates.Count == 0)
                    break;

                var domainEvents = aggregates
                    .SelectMany(aggregate => aggregate.DomainEvents)
                    .ToList();

                foreach (var aggregate in aggregates)
                {
                    aggregate.ClearDomainEvents();
                }

                await _domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);
            }

            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
