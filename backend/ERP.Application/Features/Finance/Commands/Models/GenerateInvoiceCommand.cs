using MediatR;
using System;

namespace ERP.Application.Features.Finance.Commands.Models;

public sealed record GenerateInvoiceCommand(
    int OrderId,
    DateTime DueDate
) : IRequest<int>;
