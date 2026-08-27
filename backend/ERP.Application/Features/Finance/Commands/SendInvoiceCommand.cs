namespace ERP.Application.Features.Finance.Commands;

using MediatR;

public sealed record SendInvoiceCommand(int InvoiceId) : IRequest<bool>;
