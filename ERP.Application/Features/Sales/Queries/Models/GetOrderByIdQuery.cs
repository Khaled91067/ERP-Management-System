using ERP.Application.Features.Sales.Dtos;
using MediatR;

namespace ERP.Application.Features.Sales.Queries.Models;

public sealed record GetOrderByIdQuery(int Id) : IRequest<OrderDto?>;
