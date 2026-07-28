
namespace ERP.Application.Features.Sales.Queries.Models;

using ERP.Application.Features.Sales.Dtos;

using MediatR;

public sealed record GetOrderByIdQuery(int Id) : IRequest<OrderDto?>;
