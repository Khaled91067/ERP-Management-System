
namespace ERP.Application.Features.Finance.Commands;

using MediatR;

public sealed record PayInvoiceCommand(
    int InvoiceId
) : IRequest<bool>;
