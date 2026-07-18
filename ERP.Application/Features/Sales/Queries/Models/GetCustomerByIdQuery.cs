using ERP.Application.Features.Sales.Dtos;
using MediatR;

namespace ERP.Application.Features.Sales.Queries.Models;

public sealed record GetCustomerByIdQuery(
    int Id
) : IRequest<CustomerDto?>;
