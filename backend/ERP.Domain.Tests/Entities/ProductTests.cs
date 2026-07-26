using ERP.Domain.Entities;
using ERP.Domain.Exceptions;
using FluentAssertions;

namespace ERP.Domain.Tests.Entities;

public class ProductTests
{
    // -----------------------------------------------------------------------
    // IncreaseStock
    // -----------------------------------------------------------------------

    [Fact]
    public void IncreaseStock_WithPositiveQuantity_IncreasesStockQuantity()
    {
        // Arrange
        var product = new Product { Name = "Widget" };
        product.IncreaseStock(10); // seed to 10

        // Act
        product.IncreaseStock(5);

        // Assert
        product.StockQuantity.Should().Be(15);
    }

    [Fact]
    public void IncreaseStock_WhenInitiallyZero_SetsStockToQuantity()
    {
        // Arrange
        var product = new Product { Name = "Widget" }; // StockQuantity defaults to 0

        // Act
        product.IncreaseStock(20);

        // Assert
        product.StockQuantity.Should().Be(20);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void IncreaseStock_WithNonPositiveQuantity_ThrowsDomainException(int invalidQuantity)
    {
        // Arrange
        var product = new Product { Name = "Widget" };

        // Act
        var act = () => product.IncreaseStock(invalidQuantity);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*Stock increase quantity must be greater than zero*");
    }

    // -----------------------------------------------------------------------
    // DecreaseStock
    // -----------------------------------------------------------------------

    [Fact]
    public void DecreaseStock_WithSufficientStock_DecreasesStockQuantity()
    {
        // Arrange
        var product = new Product { Name = "Widget" };
        product.IncreaseStock(20);

        // Act
        product.DecreaseStock(8);

        // Assert
        product.StockQuantity.Should().Be(12);
    }

    [Fact]
    public void DecreaseStock_ExactAvailableQuantity_SetsStockToZero()
    {
        // Arrange
        var product = new Product { Name = "Widget" };
        product.IncreaseStock(10);

        // Act
        product.DecreaseStock(10);

        // Assert
        product.StockQuantity.Should().Be(0);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void DecreaseStock_WithNonPositiveQuantity_ThrowsDomainException(int invalidQuantity)
    {
        // Arrange
        var product = new Product { Name = "Widget" };
        product.IncreaseStock(10);

        // Act
        var act = () => product.DecreaseStock(invalidQuantity);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*Stock decrease quantity must be greater than zero*");
    }

    [Fact]
    public void DecreaseStock_WithInsufficientStock_ThrowsDomainException()
    {
        // Arrange
        var product = new Product { Name = "Widget" };
        product.IncreaseStock(5);

        // Act
        var act = () => product.DecreaseStock(10);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*Insufficient stock*");
    }
}
