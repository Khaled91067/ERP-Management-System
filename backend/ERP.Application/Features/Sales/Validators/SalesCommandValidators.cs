namespace ERP.Application.Features.Sales.Validators;

using ERP.Application.Features.Sales.Commands;
using FluentValidation;

public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Customer name is required.")
            .MaximumLength(200)
            .WithMessage("Customer name cannot exceed 200 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email format is invalid.")
            .MaximumLength(256)
            .WithMessage("Email cannot exceed 256 characters.");
    }
}

public sealed class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Customer ID must be valid.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Customer name is required.")
            .MaximumLength(200)
            .WithMessage("Customer name cannot exceed 200 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email format is invalid.")
            .MaximumLength(256)
            .WithMessage("Email cannot exceed 256 characters.");
    }
}

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0)
            .WithMessage("Customer ID must be valid.");

        RuleFor(x => x.ShippingAddress)
            .NotEmpty()
            .WithMessage("Shipping address is required.")
            .MaximumLength(500)
            .WithMessage("Shipping address cannot exceed 500 characters.");

        RuleFor(x => x.PaymentMethod)
            .IsInEnum()
            .WithMessage("Payment method is invalid.");

        RuleFor(x => x.Lines)
            .NotEmpty()
            .WithMessage("Order must have at least one line item.");

        RuleForEach(x => x.Lines)
            .SetValidator(new CreateOrderLineCommandValidator());
    }
}

public sealed class CreateOrderLineCommandValidator : AbstractValidator<CreateOrderLineCommand>
{
    public CreateOrderLineCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Product ID must be valid.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Unit price cannot be negative.");

        RuleFor(x => x.DiscountPercentage)
            .InclusiveBetween(0, 100)
            .WithMessage("Discount percentage must be between 0 and 100.");
    }
}
