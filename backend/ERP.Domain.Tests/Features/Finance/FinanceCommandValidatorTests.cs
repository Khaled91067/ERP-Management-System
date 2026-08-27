namespace ERP.Domain.Tests.Features.Finance;

using System;
using System.Collections.Generic;
using ERP.Application.Features.Finance.Commands;
using ERP.Application.Features.Finance.Validators;
using FluentAssertions;
using Xunit;

public class FinanceCommandValidatorTests
{
    private static DateTime FutureDueDate => DateTime.UtcNow.Date.AddDays(14);

    // -----------------------------------------------------------------------
    // CreateInvoiceCommandValidator
    // -----------------------------------------------------------------------

    [Fact]
    public void CreateInvoiceCommandValidator_WithValidData_PassesValidation()
    {
        // Arrange
        var validator = new CreateInvoiceCommandValidator();
        var command = new CreateInvoiceCommand(
            CustomerId: 1,
            DueDate: FutureDueDate,
            Lines: new List<CreateInvoiceLineCommand>
            {
                new("Item 1", Quantity: 2, UnitPrice: 50m, TaxRate: 5m)
            },
            OrderId: null);

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CreateInvoiceCommandValidator_WithValidOrderId_PassesValidation()
    {
        // Arrange
        var validator = new CreateInvoiceCommandValidator();
        var command = new CreateInvoiceCommand(
            CustomerId: 1,
            DueDate: FutureDueDate,
            Lines: new List<CreateInvoiceLineCommand>
            {
                new("Item 1", Quantity: 1, UnitPrice: 100m, TaxRate: 0m)
            },
            OrderId: 5);

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CreateInvoiceCommandValidator_WithInvalidCustomerId_FailsValidation()
    {
        // Arrange
        var validator = new CreateInvoiceCommandValidator();
        var command = new CreateInvoiceCommand(
            CustomerId: 0,
            DueDate: FutureDueDate,
            Lines: new List<CreateInvoiceLineCommand> { new("Item", 1, 10m) });

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CustomerId");
    }

    [Fact]
    public void CreateInvoiceCommandValidator_WithPastDueDate_FailsValidation()
    {
        // Arrange
        var validator = new CreateInvoiceCommandValidator();
        var command = new CreateInvoiceCommand(
            CustomerId: 1,
            DueDate: DateTime.UtcNow.Date.AddDays(-1),
            Lines: new List<CreateInvoiceLineCommand> { new("Item", 1, 10m) });

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DueDate");
    }

    [Fact]
    public void CreateInvoiceCommandValidator_WithInvalidOrderId_FailsValidation()
    {
        // Arrange
        var validator = new CreateInvoiceCommandValidator();
        var command = new CreateInvoiceCommand(
            CustomerId: 1,
            DueDate: FutureDueDate,
            Lines: new List<CreateInvoiceLineCommand> { new("Item", 1, 10m) },
            OrderId: -1);

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "OrderId");
    }

    [Fact]
    public void CreateInvoiceCommandValidator_WithEmptyLines_FailsValidation()
    {
        // Arrange
        var validator = new CreateInvoiceCommandValidator();
        var command = new CreateInvoiceCommand(
            CustomerId: 1,
            DueDate: FutureDueDate,
            Lines: new List<CreateInvoiceLineCommand>());

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Lines");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreateInvoiceCommandValidator_WithInvalidLineQuantity_FailsValidation(int quantity)
    {
        // Arrange
        var validator = new CreateInvoiceCommandValidator();
        var command = new CreateInvoiceCommand(
            CustomerId: 1,
            DueDate: FutureDueDate,
            Lines: new List<CreateInvoiceLineCommand> { new("Item", quantity, 10m) });

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("Quantity"));
    }

    [Fact]
    public void CreateInvoiceCommandValidator_WithNegativeUnitPrice_FailsValidation()
    {
        // Arrange
        var validator = new CreateInvoiceCommandValidator();
        var command = new CreateInvoiceCommand(
            CustomerId: 1,
            DueDate: FutureDueDate,
            Lines: new List<CreateInvoiceLineCommand> { new("Item", 1, -5m) });

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("UnitPrice"));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void CreateInvoiceCommandValidator_WithInvalidTaxRate_FailsValidation(decimal taxRate)
    {
        // Arrange
        var validator = new CreateInvoiceCommandValidator();
        var command = new CreateInvoiceCommand(
            CustomerId: 1,
            DueDate: FutureDueDate,
            Lines: new List<CreateInvoiceLineCommand> { new("Item", 1, 10m, taxRate) });

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("TaxRate"));
    }

    // -----------------------------------------------------------------------
    // GenerateInvoiceCommandValidator
    // -----------------------------------------------------------------------

    [Fact]
    public void GenerateInvoiceCommandValidator_WithValidData_PassesValidation()
    {
        // Arrange
        var validator = new GenerateInvoiceCommandValidator();
        var command = new GenerateInvoiceCommand(OrderId: 1, DueDate: FutureDueDate);

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GenerateInvoiceCommandValidator_WithInvalidOrderId_FailsValidation(int orderId)
    {
        // Arrange
        var validator = new GenerateInvoiceCommandValidator();
        var command = new GenerateInvoiceCommand(OrderId: orderId, DueDate: FutureDueDate);

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "OrderId");
    }

    // -----------------------------------------------------------------------
    // UpdateInvoiceCommandValidator
    // -----------------------------------------------------------------------

    [Fact]
    public void UpdateInvoiceCommandValidator_WithValidData_PassesValidation()
    {
        // Arrange
        var validator = new UpdateInvoiceCommandValidator();
        var command = new UpdateInvoiceCommand(Id: 1, DueDate: FutureDueDate);

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void UpdateInvoiceCommandValidator_WithInvalidId_FailsValidation(int id)
    {
        // Arrange
        var validator = new UpdateInvoiceCommandValidator();
        var command = new UpdateInvoiceCommand(Id: id, DueDate: FutureDueDate);

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }
}

