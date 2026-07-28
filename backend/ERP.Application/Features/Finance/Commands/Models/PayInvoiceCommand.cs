
namespace ERP.Application.Features.Finance.Commands.Models;

using MediatR;

public sealed record PayInvoiceCommand(
    int InvoiceId
) : IRequest<bool>;
