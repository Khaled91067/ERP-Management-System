using ERP.Application.Features.Finance.Dtos;
using MediatR;
using System.Collections.Generic;

namespace ERP.Application.Features.Finance.Queries.Models;

public sealed record GetInvoicesQuery(
    int? CustomerId = null,
    string? Status = null
) : IRequest<IEnumerable<InvoiceDto>>;
