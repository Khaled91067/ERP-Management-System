using ERP.Application.Features.Sales.Commands.Models;
using ERP.Application.Features.Sales.Queries.Models;
using ERP.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly ISender _sender;

    public OrdersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? customerId,
        [FromQuery] string? searchTerm,
        CancellationToken cancellationToken)
    {
        var query = new GetOrdersQuery(customerId, searchTerm);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var query = new GetOrderByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<PaymentMethod>(request.PaymentMethod, true, out var paymentMethod))
        {
            return BadRequest("Invalid payment method. Allowed values: Cash, CreditCard, MobilePayment");
        }

        var command = new CreateOrderCommand(
            request.CustomerId,
            paymentMethod,
            request.ShippingAddress,
            request.Lines.Select(l => new CreateOrderLineCommand(
                l.ProductId,
                l.Quantity,
                l.UnitPrice,
                l.DiscountPercentage
            )).ToList()
        );

        var id = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateOrderStatusCommand command, CancellationToken cancellationToken)
    {
        if (id != command.OrderId)
            return BadRequest("Order ID mismatch in request body and URL.");

        var result = await _sender.Send(command, cancellationToken);
        return result ? NoContent() : NotFound();
    }
}

public sealed record CreateOrderRequest(
    int CustomerId,
    string PaymentMethod,
    string ShippingAddress,
    List<CreateOrderLineRequest> Lines);

public sealed record CreateOrderLineRequest(
    int ProductId,
    int Quantity,
    decimal UnitPrice,
    decimal DiscountPercentage = 0);
