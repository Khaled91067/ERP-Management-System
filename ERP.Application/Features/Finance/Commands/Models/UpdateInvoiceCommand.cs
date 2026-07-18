using MediatR;

namespace ERP.Application.Features.Finance.Commands.Models;

public sealed record UpdateInvoiceCommand(
    int Id,
    DateTime DueDate) : IRequest<bool>;
