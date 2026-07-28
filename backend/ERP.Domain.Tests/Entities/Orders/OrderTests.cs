
namespace ERP.Domain.Tests.Entities.Orders;

using ERP.Domain.Sales.Orders;
using ERP.Domain.Shared.Exceptions;

using FluentAssertions;

public class OrderTests
{
    // -----------------------------------------------------------------------
    // Constructor
    // -----------------------------------------------------------------------

    [Fact]
    public void Constructor_WithValidArguments_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var order = new Order(customerId: 1, PaymentMethod.Cash, "123 Main St");

        // Assert
        order.CustomerId.Should().Be(1);
        order.PaymentMethod.Should().Be(PaymentMethod.Cash);
        order.ShippingAddress.Should().Be("123 Main St");
        order.Status.Should().Be(OrderStatus.Pending);
        order.TotalAmount.Amount.Should().Be(0);
        order.OrderDate.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithInvalidCustomerId_ThrowsDomainException(int invalidId)
    {
        // Arrange & Act
        var act = () => new Order(customerId: invalidId, PaymentMethod.Cash, "123 Main St");

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Customer id must be valid*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null!)]
    public void Constructor_WithEmptyShippingAddress_ThrowsDomainException(string? address)
    {
        // Arrange & Act
        var act = () => new Order(customerId: 1, PaymentMethod.Cash, address!);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Shipping address is required*");
    }

    [Fact]
    public void Constructor_TrimsShippingAddress()
    {
        // Arrange & Act
        var order = new Order(1, PaymentMethod.Cash, "  42 Elm Street  ");

        // Assert
        order.ShippingAddress.Should().Be("42 Elm Street");
    }

    // -----------------------------------------------------------------------
    // AddLine
    // -----------------------------------------------------------------------

    [Fact]
    public void AddLine_WithValidArguments_AddsLineAndRecalculatesTotal()
    {
        // Arrange
        var order = new Order(1, PaymentMethod.Cash, "123 Main St");

        // Act
        order.AddLine(productId: 1, quantity: 2, unitPrice: 50m);

        // Assert
        order.OrderLines.Should().HaveCount(1);
        order.TotalAmount.Amount.Should().Be(100m); // 2 * 50 * (1 - 0/100)
    }

    [Fact]
    public void AddLine_WithDiscount_CalculatesTotalAfterDiscount()
    {
        // Arrange
        var order = new Order(1, PaymentMethod.Cash, "123 Main St");

        // Act
        order.AddLine(productId: 1, quantity: 4, unitPrice: 100m, discountPercentage: 25m);

        // Assert
        // (4 * 100) * (1 - 25/100) = 400 * 0.75 = 300
        order.TotalAmount.Amount.Should().Be(300m);
    }

    [Fact]
    public void AddLine_MultipleLines_AccumulatesTotal()
    {
        // Arrange
        var order = new Order(1, PaymentMethod.Cash, "123 Main St");

        // Act
        order.AddLine(productId: 1, quantity: 2, unitPrice: 50m);   // 100
        order.AddLine(productId: 2, quantity: 1, unitPrice: 200m);  // 200

        // Assert
        order.OrderLines.Should().HaveCount(2);
        order.TotalAmount.Amount.Should().Be(300m);
    }

    [Fact]
    public void AddLine_WithDuplicateProduct_ThrowsDomainException()
    {
        // Arrange
        var order = new Order(1, PaymentMethod.Cash, "123 Main St");
        order.AddLine(productId: 1, quantity: 1, unitPrice: 10m);

        // Act
        var act = () => order.AddLine(productId: 1, quantity: 2, unitPrice: 20m);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Product already exists in this order*");
    }

    [Fact]
    public void AddLine_WhenOrderIsNotPending_ThrowsDomainException()
    {
        // Arrange
        var order = new Order(1, PaymentMethod.Cash, "123 Main St");
        order.AddLine(productId: 1, quantity: 1, unitPrice: 10m);
        order.Ship();

        // Act
        var act = () => order.AddLine(productId: 2, quantity: 1, unitPrice: 10m);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Only pending orders can be modified*");
    }

    // -----------------------------------------------------------------------
    // RemoveLine
    // -----------------------------------------------------------------------

    [Fact]
    public void RemoveLine_ExistingProduct_RemovesLineAndRecalculatesTotal()
    {
        // Arrange
        var order = new Order(1, PaymentMethod.Cash, "123 Main St");
        order.AddLine(productId: 1, quantity: 2, unitPrice: 50m);  // 100
        order.AddLine(productId: 2, quantity: 1, unitPrice: 30m);  // 30

        // Act
        order.RemoveLine(productId: 1);

        // Assert
        order.OrderLines.Should().HaveCount(1);
        order.TotalAmount.Amount.Should().Be(30m);
    }

    [Fact]
    public void RemoveLine_NonExistentProduct_ThrowsDomainException()
    {
        // Arrange
        var order = new Order(1, PaymentMethod.Cash, "123 Main St");

        // Act
        var act = () => order.RemoveLine(productId: 99);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Order line was not found*");
    }

    [Fact]
    public void RemoveLine_WhenOrderIsNotPending_ThrowsDomainException()
    {
        // Arrange
        var order = new Order(1, PaymentMethod.Cash, "123 Main St");
        order.AddLine(productId: 1, quantity: 1, unitPrice: 10m);
        order.Ship();

        // Act
        var act = () => order.RemoveLine(productId: 1);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Only pending orders can be modified*");
    }

    // -----------------------------------------------------------------------
    // Ship
    // -----------------------------------------------------------------------

    [Fact]
    public void Ship_WhenPendingWithLines_TransitionsToShipped()
    {
        // Arrange
        var order = new Order(1, PaymentMethod.Cash, "123 Main St");
        order.AddLine(productId: 1, quantity: 1, unitPrice: 10m);

        // Act
        order.Ship();

        // Assert
        order.Status.Should().Be(OrderStatus.Shipped);
    }

    [Fact]
    public void Ship_WhenOrderIsEmpty_ThrowsDomainException()
    {
        // Arrange
        var order = new Order(1, PaymentMethod.Cash, "123 Main St");

        // Act
        var act = () => order.Ship();

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Cannot ship an empty order*");
    }

    [Fact]
    public void Ship_WhenOrderIsNotPending_ThrowsDomainException()
    {
        // Arrange
        var order = new Order(1, PaymentMethod.Cash, "123 Main St");
        order.AddLine(productId: 1, quantity: 1, unitPrice: 10m);
        order.Ship();

        // Act
        var act = () => order.Ship();

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Only pending orders can be shipped*");
    }

    // -----------------------------------------------------------------------
    // Deliver
    // -----------------------------------------------------------------------

    [Fact]
    public void Deliver_WhenShipped_TransitionsToDelivered()
    {
        // Arrange
        var order = new Order(1, PaymentMethod.Cash, "123 Main St");
        order.AddLine(productId: 1, quantity: 1, unitPrice: 10m);
        order.Ship();

        // Act
        order.Deliver();

        // Assert
        order.Status.Should().Be(OrderStatus.Delivered);
    }

    [Fact]
    public void Deliver_WhenNotShipped_ThrowsDomainException()
    {
        // Arrange
        var order = new Order(1, PaymentMethod.Cash, "123 Main St");

        // Act
        var act = () => order.Deliver();

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Only shipped orders can be delivered*");
    }

    // -----------------------------------------------------------------------
    // Cancel
    // -----------------------------------------------------------------------

    [Fact]
    public void Cancel_WhenPending_TransitionsToCancelled()
    {
        // Arrange
        var order = new Order(1, PaymentMethod.Cash, "123 Main St");

        // Act
        order.Cancel();

        // Assert
        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WhenShipped_TransitionsToCancelled()
    {
        // Arrange
        var order = new Order(1, PaymentMethod.Cash, "123 Main St");
        order.AddLine(productId: 1, quantity: 1, unitPrice: 10m);
        order.Ship();

        // Act
        order.Cancel();

        // Assert
        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WhenDelivered_ThrowsDomainException()
    {
        // Arrange
        var order = new Order(1, PaymentMethod.Cash, "123 Main St");
        order.AddLine(productId: 1, quantity: 1, unitPrice: 10m);
        order.Ship();
        order.Deliver();

        // Act
        var act = () => order.Cancel();

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Delivered orders cannot be cancelled*");
    }

    [Fact]
    public void Cancel_WhenAlreadyCancelled_ThrowsDomainException()
    {
        // Arrange
        var order = new Order(1, PaymentMethod.Cash, "123 Main St");
        order.Cancel();

        // Act
        var act = () => order.Cancel();

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Order is already cancelled*");
    }
}
