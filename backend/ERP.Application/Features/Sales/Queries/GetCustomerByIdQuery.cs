
namespace ERP.Application.Features.Sales.Queries;

using global::ERP.Application.Features.Sales.Dtos;

using MediatR;

public sealed record GetCustomerByIdQuery(
    int Id
) : IRequest<CustomerDto?>;

