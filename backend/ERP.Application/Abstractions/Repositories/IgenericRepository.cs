using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Application.Abstractions.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync(QueryOptions<T>? options=null);
        Task<ERP.Application.Common.Models.PagedResult<T>> GetPagedAsync(QueryOptions<T>? options = null, int page = 1, int pageSize = 20);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
