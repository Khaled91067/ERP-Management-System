using MediatR;

namespace ERP.Application.Features.Finance.Commands.Models;

public sealed record CreateInvoiceCommand(
    int CustomerId,
    DateTime DueDate,
    List<CreateInvoiceLineCommand> Lines) : IRequest<int>;

public sealed record CreateInvoiceLineCommand(
    string Description,
    int Quantity,
    decimal UnitPrice,
    decimal TaxRate = 0);
