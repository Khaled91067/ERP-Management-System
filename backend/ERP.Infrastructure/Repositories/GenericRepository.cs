
namespace ERP.Infrastructure.Repositories
{
    using System.Linq;

    using ERP.Application.Abstractions.Repositories;
    using ERP.Infrastructure.Persistence;

    using Microsoft.EntityFrameworkCore;

    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync(QueryOptions<T>? options = null, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet;

            if (options?.AsNoTracking == true)
            {
                query = query.AsNoTracking();
            }

            if(options != null)
            {
                if (options.IncludeDeleted)
                {
                    query = query.IgnoreQueryFilters();
                }

                foreach (var filter in options.Filters)
                {
                    query = query.Where(filter);
                }
               
                foreach (var include in options.Includes)
                {
                    query = query.Include(include);
                }

                if (options.OrderBy != null)
                {
                    query = options.OrderBy(query);
                }
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<ERP.Application.Common.Models.PagedResult<T>> GetPagedAsync(QueryOptions<T>? options = null, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet;

            if (options?.AsNoTracking == true)
            {
                query = query.AsNoTracking();
            }

            if (options != null)
            {
                if (options.IncludeDeleted)
                {
                    query = query.IgnoreQueryFilters();
                }

                foreach (var filter in options.Filters)
                {
                    query = query.Where(filter);
                }

                foreach (var include in options.Includes)
                {
                    query = query.Include(include);
                }

                if (options.OrderBy != null)
                {
                    query = options.OrderBy(query);
                }
            }
            if (options?.OrderBy == null)
            {
                throw new InvalidOperationException(
                    "Paged queries require an OrderBy clause.");
            }

            query = options.OrderBy(query);
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new ERP.Application.Common.Models.PagedResult<T>(items, totalCount, page, pageSize);
        }

        public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
    }
}
