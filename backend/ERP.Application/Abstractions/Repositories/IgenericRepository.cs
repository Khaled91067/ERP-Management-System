
namespace ERP.Application.Abstractions.Repositories
{
    using System.Collections.Generic;

    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(QueryOptions<T>? options = null, CancellationToken cancellationToken = default);
        Task<ERP.Application.Common.Models.PagedResult<T>> GetPagedAsync(QueryOptions<T>? options = null, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
