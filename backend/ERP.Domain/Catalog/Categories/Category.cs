namespace ERP.Domain.Catalog.Categories;

using System.Collections.Generic;

using ERP.Domain.Catalog.Products;
using ERP.Domain.Shared.Base;
using ERP.Domain.Shared.Exceptions;

public class Category : SoftDeletableEntity
{
    private readonly List<Product> _products = [];

    public string Name { get; private set; } = string.Empty;


    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    private Category() { }

    public Category(string name)
    {
        UpdateName(name);
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleValidationException("Category name is required.");

        Name = name.Trim();
    }
}
