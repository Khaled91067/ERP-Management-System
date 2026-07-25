using ERP.Application.Abstractions.Repositories;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ERP.Infrastructure.Repositories
{
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

        public async Task<IEnumerable<T>> GetAllAsync(QueryOptions<T>? options = null)
        {
            IQueryable<T> query = _dbSet;

            if(options != null)
            {
                if (options.IncludeDeleted)
                {
                    query = query.IgnoreQueryFilters();
                }

                if(options.Filter != null)
                {
                    query = query.Where(options.Filter);

                }
               
                foreach (var include in options.Includes)
                {
                    query = query.Include(include);
                }

            }

            return await query.ToListAsync();
        }

        public async Task<ERP.Application.Common.Models.PagedResult<T>> GetPagedAsync(QueryOptions<T>? options = null, int page = 1, int pageSize = 20)
        {
            IQueryable<T> query = _dbSet;

            if (options != null)
            {
                if (options.IncludeDeleted)
                {
                    query = query.IgnoreQueryFilters();
                }

                if (options.Filter != null)
                {
                    query = query.Where(options.Filter);
                }

                foreach (var include in options.Includes)
                {
                    query = query.Include(include);
                }
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new ERP.Application.Common.Models.PagedResult<T>(items, totalCount, page, pageSize);
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
    }
}
