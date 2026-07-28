
namespace ERP.Domain.Tests.Entities.Purchasing;

using ERP.Domain.Purchasing.PurchaseOrders;
using ERP.Domain.Shared.Exceptions;

using FluentAssertions;

public class PurchaseOrderTests
{
    private static DateTime FutureDelivery => DateTime.UtcNow.AddDays(7);

    // -----------------------------------------------------------------------
    // Constructor
    // -----------------------------------------------------------------------

    [Fact]
    public void Constructor_WithValidArguments_SetsPropertiesCorrectly()
    {
        // Arrange
        var expectedDelivery = FutureDelivery;

        // Act
        var po = new PurchaseOrder(supplierId: 1, expectedDelivery);

        // Assert
        po.SupplierId.Should().Be(1);
        po.ExpectedDelivery.Should().Be(expectedDelivery);
        po.Status.Should().Be(PurchaseOrderStatus.Draft);
        po.TotalAmount.Should().Be(0);
        po.OrderDate.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Constructor_WithInvalidSupplierId_ThrowsDomainException(int invalidId)
    {
        // Arrange & Act
        var act = () => new PurchaseOrder(supplierId: invalidId, FutureDelivery);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Supplier id must be valid*");
    }

    [Fact]
    public void Constructor_WithPastExpectedDelivery_ThrowsDomainException()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddDays(-1);

        // Act
        var act = () => new PurchaseOrder(supplierId: 1, pastDate);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Expected delivery cannot be before order date*");
    }

    // -----------------------------------------------------------------------
    // AddLine
    // -----------------------------------------------------------------------

    [Fact]
    public void AddLine_WithValidArguments_AddsLineAndRecalculatesTotal()
    {
        // Arrange
        var po = new PurchaseOrder(1, FutureDelivery);

        // Act
        po.AddLine(productId: 1, quantity: 10, unitCost: 25m);

        // Assert
        po.PurchaseLines.Should().HaveCount(1);
        po.TotalAmount.Should().Be(250m); // 10 * 25
    }

    [Fact]
    public void AddLine_MultipleLines_AccumulatesTotal()
    {
        // Arrange
        var po = new PurchaseOrder(1, FutureDelivery);

        // Act
        po.AddLine(productId: 1, quantity: 10, unitCost: 25m);  // 250
        po.AddLine(productId: 2, quantity: 5, unitCost: 40m);   // 200

        // Assert
        po.PurchaseLines.Should().HaveCount(2);
        po.TotalAmount.Should().Be(450m);
    }

    [Fact]
    public void AddLine_WithDuplicateProduct_ThrowsDomainException()
    {
        // Arrange
        var po = new PurchaseOrder(1, FutureDelivery);
        po.AddLine(productId: 1, quantity: 5, unitCost: 10m);

        // Act
        var act = () => po.AddLine(productId: 1, quantity: 3, unitCost: 15m);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Product already exists in this purchase order*");
    }

    [Fact]
    public void AddLine_WhenNotDraft_ThrowsDomainException()
    {
        // Arrange
        var po = new PurchaseOrder(1, FutureDelivery);
        po.AddLine(productId: 1, quantity: 1, unitCost: 10m);
        po.Submit();

        // Act
        var act = () => po.AddLine(productId: 2, quantity: 1, unitCost: 10m);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Only draft purchase orders can be modified*");
    }

    // -----------------------------------------------------------------------
    // RemoveLine
    // -----------------------------------------------------------------------

    [Fact]
    public void RemoveLine_ExistingProduct_RemovesLineAndRecalculatesTotal()
    {
        // Arrange
        var po = new PurchaseOrder(1, FutureDelivery);
        po.AddLine(productId: 1, quantity: 10, unitCost: 25m);  // 250
        po.AddLine(productId: 2, quantity: 4, unitCost: 50m);   // 200

        // Act
        po.RemoveLine(productId: 1);

        // Assert
        po.PurchaseLines.Should().HaveCount(1);
        po.TotalAmount.Should().Be(200m);
    }

    [Fact]
    public void RemoveLine_NonExistentProduct_ThrowsDomainException()
    {
        // Arrange
        var po = new PurchaseOrder(1, FutureDelivery);

        // Act
        var act = () => po.RemoveLine(productId: 99);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Purchase line was not found*");
    }

    [Fact]
    public void RemoveLine_WhenNotDraft_ThrowsDomainException()
    {
        // Arrange
        var po = new PurchaseOrder(1, FutureDelivery);
        po.AddLine(productId: 1, quantity: 1, unitCost: 10m);
        po.Submit();

        // Act
        var act = () => po.RemoveLine(productId: 1);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Only draft purchase orders can be modified*");
    }

    // -----------------------------------------------------------------------
    // ChangeLineQuantity
    // -----------------------------------------------------------------------

    [Fact]
    public void ChangeLineQuantity_ValidQuantity_UpdatesLineAndRecalculatesTotal()
    {
        // Arrange
        var po = new PurchaseOrder(1, FutureDelivery);
        po.AddLine(productId: 1, quantity: 5, unitCost: 20m);  // 100

        // Act
        po.ChangeLineQuantity(productId: 1, quantity: 10);

        // Assert
        po.TotalAmount.Should().Be(200m); // 10 * 20
        po.PurchaseLines.Single().Quantity.Should().Be(10);
    }

    [Fact]
    public void ChangeLineQuantity_WhenNotDraft_ThrowsDomainException()
    {
        // Arrange
        var po = new PurchaseOrder(1, FutureDelivery);
        po.AddLine(productId: 1, quantity: 5, unitCost: 20m);
        po.Submit();

        // Act
        var act = () => po.ChangeLineQuantity(productId: 1, quantity: 10);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Only draft purchase orders can be modified*");
    }

    // -----------------------------------------------------------------------
    // ChangeLineUnitCost
    // -----------------------------------------------------------------------

    [Fact]
    public void ChangeLineUnitCost_ValidCost_UpdatesLineAndRecalculatesTotal()
    {
        // Arrange
        var po = new PurchaseOrder(1, FutureDelivery);
        po.AddLine(productId: 1, quantity: 5, unitCost: 20m);  // 100

        // Act
        po.ChangeLineUnitCost(productId: 1, unitCost: 30m);

        // Assert
        po.TotalAmount.Should().Be(150m); // 5 * 30
        po.PurchaseLines.Single().UnitCost.Should().Be(30m);
    }

    // -----------------------------------------------------------------------
    // Submit
    // -----------------------------------------------------------------------

    [Fact]
    public void Submit_WhenDraftWithLines_TransitionsToSubmitted()
    {
        // Arrange
        var po = new PurchaseOrder(1, FutureDelivery);
        po.AddLine(productId: 1, quantity: 1, unitCost: 10m);

        // Act
        po.Submit();

        // Assert
        po.Status.Should().Be(PurchaseOrderStatus.Submitted);
    }

    [Fact]
    public void Submit_WhenEmpty_ThrowsDomainException()
    {
        // Arrange
        var po = new PurchaseOrder(1, FutureDelivery);

        // Act
        var act = () => po.Submit();

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Cannot submit an empty purchase order*");
    }

    [Fact]
    public void Submit_WhenNotDraft_ThrowsDomainException()
    {
        // Arrange
        var po = new PurchaseOrder(1, FutureDelivery);
        po.AddLine(productId: 1, quantity: 1, unitCost: 10m);
        po.Submit();

        // Act
        var act = () => po.Submit();

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Only draft purchase orders can be submitted*");
    }

    // -----------------------------------------------------------------------
    // Approve
    // -----------------------------------------------------------------------

    [Fact]
    public void Approve_WhenDraftWithLines_TransitionsToApproved()
    {
        // Arrange
        var po = new PurchaseOrder(1, FutureDelivery);
        po.AddLine(productId: 1, quantity: 1, unitCost: 10m);

        // Act
        po.Approve();

        // Assert
        po.Status.Should().Be(PurchaseOrderStatus.Approved);
    }

    [Fact]
    public void Approve_WhenEmpty_ThrowsDomainException()
    {
        // Arrange
        var po = new PurchaseOrder(1, FutureDelivery);

        // Act
        var act = () => po.Approve();

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Cannot approve an empty purchase order*");
    }

    [Fact]
    public void Approve_WhenNotDraft_ThrowsDomainException()
    {
        // Arrange
        var po = new PurchaseOrder(1, FutureDelivery);
        po.AddLine(productId: 1, quantity: 1, unitCost: 10m);
        po.Submit();

        // Act
        var act = () => po.Approve();

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Only draft purchase orders can be approved*");
    }

    // -----------------------------------------------------------------------
    // Receive
    // -----------------------------------------------------------------------

    [Fact]
    public void Receive_WhenApproved_TransitionsToReceivedAndRaisesDomainEvent()
    {
        // Arrange
        var po = new PurchaseOrder(1, FutureDelivery);
        po.AddLine(productId: 1, quantity: 1, unitCost: 10m);
        po.Approve();

        // Act
        po.Receive();

        // Assert
        po.Status.Should().Be(PurchaseOrderStatus.Received);
        po.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<PurchaseOrderReceivedDomainEvent>();
    }

    [Fact]
    public void Receive_WhenNotApproved_ThrowsDomainException()
    {
        // Arrange
        var po = new PurchaseOrder(1, FutureDelivery);
        po.AddLine(productId: 1, quantity: 1, unitCost: 10m);

        // Act
        var act = () => po.Receive();

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Only approved purchase orders can be received*");
    }

    // -----------------------------------------------------------------------
    // Cancel
    // -----------------------------------------------------------------------

    [Fact]
    public void Cancel_WhenDraft_TransitionsToCancelled()
    {
        // Arrange
        var po = new PurchaseOrder(1, FutureDelivery);

        // Act
        po.Cancel();

        // Assert
        po.Status.Should().Be(PurchaseOrderStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WhenReceived_ThrowsDomainException()
    {
        // Arrange
        var po = new PurchaseOrder(1, FutureDelivery);
        po.AddLine(productId: 1, quantity: 1, unitCost: 10m);
        po.Approve();
        po.Receive();

        // Act
        var act = () => po.Cancel();

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Received purchase orders cannot be cancelled*");
    }

    [Fact]
    public void Cancel_WhenAlreadyCancelled_ThrowsDomainException()
    {
        // Arrange
        var po = new PurchaseOrder(1, FutureDelivery);
        po.Cancel();

        // Act
        var act = () => po.Cancel();

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*Purchase order is already cancelled*");
    }
}
