using ERP.Domain.Entities;

namespace ERP.Application.Abstractions.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdWithCategoryAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetAllWithCategoriesAsync(CancellationToken cancellationToken = default);
    Task<bool> HasTransactionsAsync(int productId, CancellationToken cancellationToken = default);
}
