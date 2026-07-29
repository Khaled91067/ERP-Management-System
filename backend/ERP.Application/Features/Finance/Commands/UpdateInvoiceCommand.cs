
namespace ERP.Application.Features.Finance.Commands;

using MediatR;

public sealed record UpdateInvoiceCommand(
    int Id,
    DateTime DueDate) : IRequest<bool>;
