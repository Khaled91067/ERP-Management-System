using ERP.Application.Abstractions.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Repositories;

public sealed class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default) =>
        _context.Categories.FirstOrDefaultAsync(category => category.Name == name, cancellationToken);

    public Task<bool> HasProductsAsync(int categoryId, CancellationToken cancellationToken = default) =>
        _context.Products.AnyAsync(product => product.CategoryId == categoryId, cancellationToken);
}
