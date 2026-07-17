using ERP.Application.Abstractions.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Repositories;

public sealed class ProductRepository : GenericRepository<Product>, IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default) =>
        _context.Products.FirstOrDefaultAsync(product => product.Sku == sku, cancellationToken);

    public Task<Product?> GetByIdWithCategoryAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Products.Include(product => product.Category)
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Product>> GetAllWithCategoriesAsync(CancellationToken cancellationToken = default) =>
        await _context.Products.Include(product => product.Category)
            .OrderBy(product => product.Name)
            .ToListAsync(cancellationToken);

    public async Task<bool> HasTransactionsAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await _context.OrderLines.AnyAsync(line => line.ProductId == productId, cancellationToken)
            || await _context.PurchaseLines.AnyAsync(line => line.ProductId == productId, cancellationToken);
    }
}
