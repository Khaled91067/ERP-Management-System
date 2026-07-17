using MediatR;

namespace ERP.Application.Features.Finance.Commands.Models;

public sealed record PayInvoiceCommand(
    int InvoiceId
) : IRequest<bool>;
