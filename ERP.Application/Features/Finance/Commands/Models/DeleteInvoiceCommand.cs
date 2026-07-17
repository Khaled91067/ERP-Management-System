using MediatR;

namespace ERP.Application.Features.Finance.Commands.Models;

public sealed record DeleteInvoiceCommand(
    int InvoiceId
) : IRequest<bool>;
