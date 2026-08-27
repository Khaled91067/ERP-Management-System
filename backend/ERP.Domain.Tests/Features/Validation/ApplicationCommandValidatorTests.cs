namespace ERP.Domain.Tests.Features.Validation;

using System;
using System.Collections.Generic;
using ERP.Application.Features.Authentication.Commands;
using ERP.Application.Features.Authentication.Validators;
using ERP.Application.Features.Catalog.Commands;
using ERP.Application.Features.Catalog.Validators;
using ERP.Application.Features.HR.Commands;
using ERP.Application.Features.HR.Validators;
using ERP.Application.Features.Identity.Commands;
using ERP.Application.Features.Identity.Validators;
using ERP.Application.Features.PurchaseOrders.Commands;
using ERP.Application.Features.PurchaseOrders.Validators;
using ERP.Application.Features.Sales.Commands;
using ERP.Application.Features.Sales.Validators;
using ERP.Application.Features.Suppliers.Commands;
using ERP.Application.Features.Suppliers.Validators;
using ERP.Domain.Sales.Orders;
using FluentAssertions;
using Xunit;

public class ApplicationCommandValidatorTests
{
    // =======================================================================
    // Authentication
    // =======================================================================

    [Fact]
    public void LoginCommandValidator_WithValidData_PassesValidation()
    {
        var validator = new LoginCommandValidator();
        var command = new LoginCommand("user@example.com", "password123");
        var result = validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "password", "Email")]
    [InlineData("not-an-email", "password", "Email")]
    [InlineData("user@example.com", "", "Password")]
    public void LoginCommandValidator_WithInvalidData_FailsValidation(string email, string password, string expectedErrorField)
    {
        var validator = new LoginCommandValidator();
        var command = new LoginCommand(email, password);
        var result = validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == expectedErrorField);
    }

    [Fact]
    public void RegisterUserCommandValidator_WithValidData_PassesValidation()
    {
        var validator = new RegisterUserCommandValidator();
        var command = new RegisterUserCommand("John", "Doe", "john.doe@example.com", "SecurePass123!");
        var result = validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "Doe", "john@example.com", "pass", "FirstName")]
    [InlineData("John", "", "john@example.com", "pass", "LastName")]
    [InlineData("John", "Doe", "invalid-email", "pass", "Email")]
    [InlineData("John", "Doe", "john@example.com", "", "Password")]
    public void RegisterUserCommandValidator_WithInvalidData_FailsValidation(string first, string last, string email, string pass, string expectedErrorField)
    {
        var validator = new RegisterUserCommandValidator();
        var command = new RegisterUserCommand(first, last, email, pass);
        var result = validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == expectedErrorField);
    }

    // =======================================================================
    // Catalog
    // =======================================================================

    [Fact]
    public void CreateProductCommandValidator_WithValidData_PassesValidation()
    {
        var validator = new CreateProductCommandValidator();
        var command = new CreateProductCommand("Widget", "WDG-001", 1, 100m, 50m, 20, 5);
        var result = validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "SKU1", 1, 10, 5, 0, 0, "Name")]
    [InlineData("Name", "", 1, 10, 5, 0, 0, "Sku")]
    [InlineData("Name", "SKU1", 0, 10, 5, 0, 0, "CategoryId")]
    [InlineData("Name", "SKU1", 1, -1, 5, 0, 0, "UnitPrice")]
    [InlineData("Name", "SKU1", 1, 10, -5, 0, 0, "CostPrice")]
    [InlineData("Name", "SKU1", 1, 10, 5, -1, 0, "InitialStockQuantity")]
    [InlineData("Name", "SKU1", 1, 10, 5, 0, -1, "ReorderLevel")]
    public void CreateProductCommandValidator_WithInvalidData_FailsValidation(
        string name, string sku, int catId, decimal unitPrice, decimal costPrice, int stock, int reorder, string expectedField)
    {
        var validator = new CreateProductCommandValidator();
        var command = new CreateProductCommand(name, sku, catId, unitPrice, costPrice, stock, reorder);
        var result = validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == expectedField);
    }

    [Fact]
    public void AdjustStockCommandValidator_WithZeroQuantityChange_FailsValidation()
    {
        var validator = new AdjustStockCommandValidator();
        var command = new AdjustStockCommand(1, 0);
        var result = validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "QuantityChange");
    }

    [Fact]
    public void AdjustStockCommandValidator_WithValidQuantityChange_PassesValidation()
    {
        var validator = new AdjustStockCommandValidator();
        var command = new AdjustStockCommand(1, -5);
        var result = validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CreateCategoryCommandValidator_WithEmptyName_FailsValidation()
    {
        var validator = new CreateCategoryCommandValidator();
        var command = new CreateCategoryCommand("");
        var result = validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    // =======================================================================
    // HR
    // =======================================================================

    [Fact]
    public void CreateDepartmentCommandValidator_WithValidData_PassesValidation()
    {
        var validator = new CreateDepartmentCommandValidator();
        var command = new CreateDepartmentCommand("Engineering");
        var result = validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CreateEmployeeCommandValidator_WithValidData_PassesValidation()
    {
        var validator = new CreateEmployeeCommandValidator();
        var command = new CreateEmployeeCommand("Alice", "Smith", "alice@example.com", "555-1234", 1, "Developer", DateTime.UtcNow, 75000m);
        var result = validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "Smith", "alice@example.com", 1, "Dev", 50000, "FirstName")]
    [InlineData("Alice", "Smith", "invalid-email", 1, "Dev", 50000, "Email")]
    [InlineData("Alice", "Smith", "alice@example.com", 0, "Dev", 50000, "DepartmentId")]
    [InlineData("Alice", "Smith", "alice@example.com", 1, "", 50000, "Position")]
    [InlineData("Alice", "Smith", "alice@example.com", 1, "Dev", -100, "Salary")]
    public void CreateEmployeeCommandValidator_WithInvalidData_FailsValidation(
        string first, string last, string email, int deptId, string pos, decimal salary, string expectedField)
    {
        var validator = new CreateEmployeeCommandValidator();
        var command = new CreateEmployeeCommand(first, last, email, "123", deptId, pos, DateTime.UtcNow, salary);
        var result = validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == expectedField);
    }

    // =======================================================================
    // Identity
    // =======================================================================

    [Fact]
    public void CreateUserCommandValidator_WithValidData_PassesValidation()
    {
        var validator = new CreateUserCommandValidator();
        var command = new CreateUserCommand("Jane", "Doe", "jane@example.com", "Secret123", 2);
        var result = validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ChangeUserRoleCommandValidator_WithInvalidRoleId_FailsValidation()
    {
        var validator = new ChangeUserRoleCommandValidator();
        var command = new ChangeUserRoleCommand(1, 0);
        var result = validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "RoleId");
    }

    [Fact]
    public void CreateRoleCommandValidator_WithEmptyName_FailsValidation()
    {
        var validator = new CreateRoleCommandValidator();
        var command = new CreateRoleCommand("", "{}");
        var result = validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    // =======================================================================
    // PurchaseOrders
    // =======================================================================

    [Fact]
    public void CreatePurchaseOrderCommandValidator_WithValidData_PassesValidation()
    {
        var validator = new CreatePurchaseOrderCommandValidator();
        var command = new CreatePurchaseOrderCommand(
            1,
            DateTime.UtcNow.AddDays(7),
            new List<CreatePurchaseOrderLineCommand>
            {
                new(1, 10, 25.5m)
            });
        var result = validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CreatePurchaseOrderCommandValidator_WithEmptyLines_FailsValidation()
    {
        var validator = new CreatePurchaseOrderCommandValidator();
        var command = new CreatePurchaseOrderCommand(1, DateTime.UtcNow.AddDays(7), new List<CreatePurchaseOrderLineCommand>());
        var result = validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Lines");
    }

    [Fact]
    public void CreatePurchaseOrderCommandValidator_WithInvalidLineQuantity_FailsValidation()
    {
        var validator = new CreatePurchaseOrderCommandValidator();
        var command = new CreatePurchaseOrderCommand(
            1,
            DateTime.UtcNow.AddDays(7),
            new List<CreatePurchaseOrderLineCommand>
            {
                new(1, 0, 25.5m)
            });
        var result = validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("Quantity"));
    }

    [Fact]
    public void UpdatePurchaseOrderStatusCommandValidator_WithEmptyStatus_FailsValidation()
    {
        var validator = new UpdatePurchaseOrderStatusCommandValidator();
        var command = new UpdatePurchaseOrderStatusCommand(1, "");
        var result = validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Status");
    }

    // =======================================================================
    // Sales
    // =======================================================================

    [Fact]
    public void CreateCustomerCommandValidator_WithValidData_PassesValidation()
    {
        var validator = new CreateCustomerCommandValidator();
        var command = new CreateCustomerCommand("Global Corp", "info@global.com", "555-0100", "100 Main", "Metropolis", "USA", "TAX-123");
        var result = validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "info@global.com", "Name")]
    [InlineData("Global Corp", "invalid-email", "Email")]
    public void CreateCustomerCommandValidator_WithInvalidData_FailsValidation(string name, string email, string expectedField)
    {
        var validator = new CreateCustomerCommandValidator();
        var command = new CreateCustomerCommand(name, email, "555", "Addr", "City", "Country", "Tax");
        var result = validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == expectedField);
    }

    [Fact]
    public void CreateOrderCommandValidator_WithValidData_PassesValidation()
    {
        var validator = new CreateOrderCommandValidator();
        var command = new CreateOrderCommand(
            CustomerId: 1,
            PaymentMethod: PaymentMethod.CreditCard,
            ShippingAddress: "456 Oak Avenue",
            Lines: new List<CreateOrderLineCommand>
            {
                new(ProductId: 1, Quantity: 2, UnitPrice: 150m, DiscountPercentage: 5m)
            });
        var result = validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CreateOrderCommandValidator_WithEmptyLines_FailsValidation()
    {
        var validator = new CreateOrderCommandValidator();
        var command = new CreateOrderCommand(1, PaymentMethod.Cash, "123 Main", new List<CreateOrderLineCommand>());
        var result = validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Lines");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void CreateOrderCommandValidator_WithInvalidDiscount_FailsValidation(decimal discount)
    {
        var validator = new CreateOrderCommandValidator();
        var command = new CreateOrderCommand(
            1,
            PaymentMethod.Cash,
            "123 Main",
            new List<CreateOrderLineCommand>
            {
                new(1, 1, 100m, discount)
            });
        var result = validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("DiscountPercentage"));
    }

    // =======================================================================
    // Suppliers
    // =======================================================================

    [Fact]
    public void CreateSupplierCommandValidator_WithValidData_PassesValidation()
    {
        var validator = new CreateSupplierCommandValidator();
        var command = new CreateSupplierCommand("Apex Supplies", "Bob", "bob@apex.com", "555-9876", "Net30");
        var result = validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "bob@apex.com", "CompanyName")]
    [InlineData("Apex Supplies", "invalid-email", "Email")]
    public void CreateSupplierCommandValidator_WithInvalidData_FailsValidation(string company, string email, string expectedField)
    {
        var validator = new CreateSupplierCommandValidator();
        var command = new CreateSupplierCommand(company, "Bob", email, "555", "Net30");
        var result = validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == expectedField);
    }
}
