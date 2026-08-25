
namespace ERP.Application.Abstractions.Repositories;

using ERP.Domain.Catalog.Categories;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> HasProductsAsync(int categoryId, CancellationToken cancellationToken = default);
}

