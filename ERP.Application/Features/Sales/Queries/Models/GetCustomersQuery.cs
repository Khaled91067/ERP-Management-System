using ERP.Application.Features.Sales.Dtos;
using MediatR;
using System.Collections.Generic;

namespace ERP.Application.Features.Sales.Queries.Models;

public sealed record GetCustomersQuery(
    string? SearchTerm = null
) : IRequest<IEnumerable<CustomerDto>>;
