using ERP.Application.Features.Finance.Dtos;
using MediatR;

namespace ERP.Application.Features.Finance.Queries.Models;

public sealed record GetInvoiceByIdQuery(
    int Id
) : IRequest<InvoiceDto?>;
