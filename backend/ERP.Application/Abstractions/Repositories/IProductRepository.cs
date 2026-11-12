namespace ERP.Application.Abstractions.Repositories;

using System.Threading;
using System.Threading.Tasks;
using ERP.Domain.Catalog.Products;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdWithCategoryAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> HasTransactionsAsync(int productId, CancellationToken cancellationToken = default);
}
