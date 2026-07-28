
namespace ERP.Domain.Tests.Entities;

using ERP.Domain.Sales.Invoices;
using ERP.Domain.Shared.Exceptions;

using FluentAssertions;

public class InvoiceTests
{
    private static DateTime FutureDueDate => DateTime.UtcNow.Date.AddDays(30);

    // -----------------------------------------------------------------------
    // Constructor
    // -----------------------------------------------------------------------

    [Fact]
    public void Constructor_WithValidArguments_SetsPropertiesCorrectly()
    {
        // Arrange
        var dueDate = FutureDueDate;

        // Act
        var invoice = new Invoice(orderId: 1, customerId: 2, dueDate);

        // Assert
        invoice.OrderId.Should().Be(1);
        invoice.CustomerId.Should().Be(2);
        invoice.DueDate.Should().Be(dueDate);
        invoice.Status.Should().Be(InvoiceStatus.Draft);
        invoice.TotalAmount.Should().Be(0);
        invoice.InvoiceDate.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithInvalidOrderId_ThrowsDomainException(int invalidId)
    {
        // Arrange & Act
        var act = () => new Invoice(orderId: invalidId, customerId: 1, FutureDueDate);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Order ID must be valid*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithInvalidCustomerId_ThrowsDomainException(int invalidId)
    {
        // Arrange & Act
        var act = () => new Invoice(orderId: 1, customerId: invalidId, FutureDueDate);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Customer ID must be valid*");
    }

    [Fact]
    public void Constructor_WithPastDueDate_ThrowsDomainException()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.Date.AddDays(-1);

        // Act
        var act = () => new Invoice(orderId: 1, customerId: 1, pastDate);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Due date cannot be in the past*");
    }

    // -----------------------------------------------------------------------
    // AddLine
    // -----------------------------------------------------------------------

    [Fact]
    public void AddLine_WithNoTax_AddsLineAndRecalculatesTotal()
    {
        // Arrange
        var invoice = new Invoice(1, 1, FutureDueDate);

        // Act
        invoice.AddLine(description: "Service A", quantity: 3, unitPrice: 100m);

        // Assert
        invoice.InvoiceLines.Should().HaveCount(1);
        invoice.TotalAmount.Should().Be(300m); // 3 * 100 * (1 + 0/100)
    }

    [Fact]
    public void AddLine_WithTax_CalculatesTotalIncludingTax()
    {
        // Arrange
        var invoice = new Invoice(1, 1, FutureDueDate);

        // Act
        invoice.AddLine(description: "Service B", quantity: 2, unitPrice: 100m, taxRate: 15m);

        // Assert
        // (2 * 100) * (1 + 15/100) = 200 * 1.15 = 230
        invoice.TotalAmount.Should().Be(230m);
    }

    [Fact]
    public void AddLine_MultipleLines_AccumulatesTotal()
    {
        // Arrange
        var invoice = new Invoice(1, 1, FutureDueDate);

        // Act
        invoice.AddLine(description: "Line 1", quantity: 1, unitPrice: 100m);
        invoice.AddLine(description: "Line 2", quantity: 2, unitPrice: 50m, taxRate: 10m);

        // Assert
        // Line 1: 100 * (1 + 0) = 100
        // Line 2: (2*50) * (1 + 0.10) = 110
        invoice.TotalAmount.Should().Be(210m);
    }

    [Fact]
    public void AddLine_WhenInvoiceIsNotDraft_ThrowsDomainException()
    {
        // Arrange
        var invoice = new Invoice(1, 1, FutureDueDate);
        invoice.AddLine(description: "Service A", quantity: 1, unitPrice: 50m);
        invoice.Send();

        // Act
        var act = () => invoice.AddLine(description: "Service B", quantity: 1, unitPrice: 50m);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Lines can only be added to draft invoices*");
    }

    // -----------------------------------------------------------------------
    // Send
    // -----------------------------------------------------------------------

    [Fact]
    public void Send_WhenDraftWithLines_TransitionsToSent()
    {
        // Arrange
        var invoice = new Invoice(1, 1, FutureDueDate);
        invoice.AddLine(description: "Service A", quantity: 1, unitPrice: 50m);

        // Act
        invoice.Send();

        // Assert
        invoice.Status.Should().Be(InvoiceStatus.Sent);
    }

    [Fact]
    public void Send_WhenEmpty_ThrowsDomainException()
    {
        // Arrange
        var invoice = new Invoice(1, 1, FutureDueDate);

        // Act
        var act = () => invoice.Send();

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Cannot send an empty invoice*");
    }

    [Fact]
    public void Send_WhenNotDraft_ThrowsDomainException()
    {
        // Arrange
        var invoice = new Invoice(1, 1, FutureDueDate);
        invoice.AddLine(description: "Service A", quantity: 1, unitPrice: 50m);
        invoice.Send();

        // Act
        var act = () => invoice.Send();

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Only draft invoices can be sent*");
    }

    // -----------------------------------------------------------------------
    // Pay
    // -----------------------------------------------------------------------

    [Fact]
    public void Pay_WhenSent_TransitionsToPaymentAndSetsPaidAt()
    {
        // Arrange
        var invoice = new Invoice(1, 1, FutureDueDate);
        invoice.AddLine(description: "Service A", quantity: 1, unitPrice: 50m);
        invoice.Send();

        // Act
        invoice.Pay();

        // Assert
        invoice.Status.Should().Be(InvoiceStatus.Paid);
        invoice.PaidAt.Should().NotBeNull();
        invoice.PaidAt.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Pay_WhenNotSent_ThrowsDomainException()
    {
        // Arrange
        var invoice = new Invoice(1, 1, FutureDueDate);

        // Act
        var act = () => invoice.Pay();

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Only sent invoices can be marked as paid*");
    }

    // -----------------------------------------------------------------------
    // Cancel
    // -----------------------------------------------------------------------

    [Fact]
    public void Cancel_WhenDraft_TransitionsToCancelled()
    {
        // Arrange
        var invoice = new Invoice(1, 1, FutureDueDate);

        // Act
        invoice.Cancel();

        // Assert
        invoice.Status.Should().Be(InvoiceStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WhenSent_TransitionsToCancelled()
    {
        // Arrange
        var invoice = new Invoice(1, 1, FutureDueDate);
        invoice.AddLine(description: "Service A", quantity: 1, unitPrice: 50m);
        invoice.Send();

        // Act
        invoice.Cancel();

        // Assert
        invoice.Status.Should().Be(InvoiceStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WhenPaid_ThrowsDomainException()
    {
        // Arrange
        var invoice = new Invoice(1, 1, FutureDueDate);
        invoice.AddLine(description: "Service A", quantity: 1, unitPrice: 50m);
        invoice.Send();
        invoice.Pay();

        // Act
        var act = () => invoice.Cancel();

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Paid invoices cannot be cancelled*");
    }

    [Fact]
    public void Cancel_WhenAlreadyCancelled_ThrowsDomainException()
    {
        // Arrange
        var invoice = new Invoice(1, 1, FutureDueDate);
        invoice.Cancel();

        // Act
        var act = () => invoice.Cancel();

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Invoice is already cancelled*");
    }
}
