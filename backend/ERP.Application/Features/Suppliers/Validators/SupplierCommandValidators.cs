namespace ERP.Application.Features.Suppliers.Validators;

using ERP.Application.Features.Suppliers.Commands;
using FluentValidation;

public sealed class CreateSupplierCommandValidator : AbstractValidator<CreateSupplierCommand>
{
    public CreateSupplierCommandValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .WithMessage("Supplier company name is required.")
            .MaximumLength(200)
            .WithMessage("Supplier company name cannot exceed 200 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email format is invalid.")
            .MaximumLength(256)
            .WithMessage("Email cannot exceed 256 characters.");
    }
}

public sealed class UpdateSupplierCommandValidator : AbstractValidator<UpdateSupplierCommand>
{
    public UpdateSupplierCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Supplier ID must be valid.");

        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .WithMessage("Supplier company name is required.")
            .MaximumLength(200)
            .WithMessage("Supplier company name cannot exceed 200 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email format is invalid.")
            .MaximumLength(256)
            .WithMessage("Email cannot exceed 256 characters.");
    }
}
