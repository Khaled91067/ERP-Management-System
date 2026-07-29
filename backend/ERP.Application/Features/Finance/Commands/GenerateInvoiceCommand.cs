
namespace ERP.Application.Features.Finance.Commands;

using System;

using MediatR;

public sealed record GenerateInvoiceCommand(
    int OrderId,
    DateTime DueDate
) : IRequest<int>;
