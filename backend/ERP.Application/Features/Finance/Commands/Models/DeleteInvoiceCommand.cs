
namespace ERP.Application.Features.Finance.Commands.Models;

using MediatR;

public sealed record DeleteInvoiceCommand(
    int InvoiceId
) : IRequest<bool>;
