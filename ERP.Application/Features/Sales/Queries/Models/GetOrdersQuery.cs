using ERP.Application.Features.Sales.Dtos;
using MediatR;
using System.Collections.Generic;

namespace ERP.Application.Features.Sales.Queries.Models;

public sealed record GetOrdersQuery(
    int? CustomerId = null,
    string? SearchTerm = null
) : IRequest<IEnumerable<OrderDto>>;
