namespace ERP.Application.Features.PurchaseOrders.Validators;

using ERP.Application.Features.PurchaseOrders.Commands;
using FluentValidation;

public sealed class CreatePurchaseOrderCommandValidator : AbstractValidator<CreatePurchaseOrderCommand>
{
    public CreatePurchaseOrderCommandValidator()
    {
        RuleFor(x => x.SupplierId)
            .GreaterThan(0)
            .WithMessage("Supplier ID must be valid.");

        RuleFor(x => x.Lines)
            .NotEmpty()
            .WithMessage("Purchase order must have at least one line item.");

        RuleForEach(x => x.Lines)
            .SetValidator(new CreatePurchaseOrderLineCommandValidator());
    }
}

public sealed class CreatePurchaseOrderLineCommandValidator : AbstractValidator<CreatePurchaseOrderLineCommand>
{
    public CreatePurchaseOrderLineCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Product ID must be valid.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.UnitCost)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Unit cost cannot be negative.");
    }
}

public sealed class UpdatePurchaseOrderStatusCommandValidator : AbstractValidator<UpdatePurchaseOrderStatusCommand>
{
    public UpdatePurchaseOrderStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Purchase order ID must be valid.");

        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Status is required.");
    }
}
