
namespace ERP.Application.Features.Finance.Queries.Models;

using global::ERP.Application.Common.Models;
using global::ERP.Application.Features.Finance.Dtos;

using MediatR;

public sealed record GetInvoicesQuery(
    int? CustomerId = null,
    string? Status = null,
    string? Search = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<InvoiceDto>>;

