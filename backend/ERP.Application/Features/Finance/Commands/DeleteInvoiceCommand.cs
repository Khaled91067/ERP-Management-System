
namespace ERP.Application.Features.Finance.Commands;

using MediatR;

public sealed record DeleteInvoiceCommand(
    int InvoiceId
) : IRequest<bool>;
