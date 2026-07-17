using ERP.Domain.Entities;

namespace ERP.Application.Abstractions.Repositories;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> HasProductsAsync(int categoryId, CancellationToken cancellationToken = default);
}
