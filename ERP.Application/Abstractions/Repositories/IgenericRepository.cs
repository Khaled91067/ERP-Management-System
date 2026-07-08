using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Application.Abstractions.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync(QueryOptions<T>? options=null);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
