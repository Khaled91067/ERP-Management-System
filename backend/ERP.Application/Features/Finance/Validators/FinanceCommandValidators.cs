namespace ERP.Application.Features.Finance.Validators;

using System;
using ERP.Application.Features.Finance.Commands;
using FluentValidation;

public sealed class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0)
            .WithMessage("Customer ID must be valid.");

        RuleFor(x => x.DueDate)
            .Must(d => d.Date >= DateTime.UtcNow.Date)
            .WithMessage("Due date cannot be in the past.");

        RuleFor(x => x.OrderId)
            .GreaterThan(0)
            .When(x => x.OrderId.HasValue)
            .WithMessage("Order ID must be valid when provided.");

        RuleFor(x => x.Lines)
            .NotEmpty()
            .WithMessage("Invoice must have at least one line item.");

        RuleForEach(x => x.Lines)
            .SetValidator(new CreateInvoiceLineCommandValidator());
    }
}

public sealed class CreateInvoiceLineCommandValidator : AbstractValidator<CreateInvoiceLineCommand>
{
    public CreateInvoiceLineCommandValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Invoice line description is required.")
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Unit price cannot be negative.");

        RuleFor(x => x.TaxRate)
            .InclusiveBetween(0, 100)
            .WithMessage("Tax rate must be between 0 and 100.");
    }
}

public sealed class GenerateInvoiceCommandValidator : AbstractValidator<GenerateInvoiceCommand>
{
    public GenerateInvoiceCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0)
            .WithMessage("Order ID must be valid.");

        RuleFor(x => x.DueDate)
            .Must(d => d.Date >= DateTime.UtcNow.Date)
            .WithMessage("Due date cannot be in the past.");
    }
}

public sealed class UpdateInvoiceCommandValidator : AbstractValidator<UpdateInvoiceCommand>
{
    public UpdateInvoiceCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Invoice ID must be valid.");

        RuleFor(x => x.DueDate)
            .Must(d => d.Date >= DateTime.UtcNow.Date)
            .WithMessage("Due date cannot be in the past.");
    }
}

