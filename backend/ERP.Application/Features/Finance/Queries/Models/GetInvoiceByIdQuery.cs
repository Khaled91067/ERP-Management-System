
namespace ERP.Application.Features.Finance.Queries.Models;

using ERP.Application.Features.Finance.Dtos;

using MediatR;

public sealed record GetInvoiceByIdQuery(
    int Id
) : IRequest<InvoiceDto?>;
