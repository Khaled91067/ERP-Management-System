using ERP.Application.Features.Finance.Commands.Models;
using ERP.Application.Features.Finance.Queries.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/invoices")]
public sealed class InvoicesController : ControllerBase
{
    private readonly ISender _sender;

    public InvoicesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? customerId,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var query = new GetInvoicesQuery(customerId, status);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var query = new GetInvoiceByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate(GenerateInvoiceCommand command, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}/pay")]
    public async Task<IActionResult> Pay(int id, PayInvoiceCommand command, CancellationToken cancellationToken)
    {
        if (id != command.InvoiceId)
            return BadRequest("Invoice ID mismatch between request URL and body.");

        var result = await _sender.Send(command, cancellationToken);
        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var command = new DeleteInvoiceCommand(id);
        var result = await _sender.Send(command, cancellationToken);
        return result ? NoContent() : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateInvoiceCommand command, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateInvoiceCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Invoice ID mismatch between request URL and body.");

        var result = await _sender.Send(command, cancellationToken);
        return result ? NoContent() : NotFound();
    }

    [HttpGet("{id:int}/pdf")]
    public async Task<IActionResult> GetPdf(int id, CancellationToken cancellationToken)
    {
        var query = new GetInvoicePdfQuery(id);
        var pdfBytes = await _sender.Send(query, cancellationToken);
        return File(pdfBytes, "application/pdf", $"invoice_{id}.pdf");
    }
}

