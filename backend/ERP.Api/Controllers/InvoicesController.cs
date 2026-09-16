namespace ERP.Api.Controllers;

using System.Threading;
using System.Threading.Tasks;

using ERP.Application.Features.Finance.Commands;
using ERP.Application.Features.Finance.Queries;
using ERP.Domain.Identity.Permissions;
using ERP.Infrastructure.Authentication.Permissions;

using MediatR;

using Microsoft.AspNetCore.Mvc;

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
    [HasPermission(Permissions.Invoices.Read)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? customerId,
        [FromQuery] string? status,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetInvoicesQuery(customerId, status, search, page, pageSize);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [HasPermission(Permissions.Invoices.Read)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var query = new GetInvoiceByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("generate")]
    [HasPermission(Permissions.Invoices.Write)]
    public async Task<IActionResult> Generate(GenerateInvoiceCommand command, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}/send")]
    [HasPermission(Permissions.Invoices.Send)]
    public async Task<IActionResult> Send(int id, CancellationToken cancellationToken)
    {
        var command = new SendInvoiceCommand(id);
        var result = await _sender.Send(command, cancellationToken);
        return result ? NoContent() : NotFound();
    }

    [HttpPut("{id:int}/pay")]
    [HasPermission(Permissions.Invoices.Pay)]
    public async Task<IActionResult> Pay(int id, PayInvoiceCommand command, CancellationToken cancellationToken)
    {
        if (id != command.InvoiceId)
            return BadRequest("Invoice ID mismatch between request URL and body.");

        var result = await _sender.Send(command, cancellationToken);
        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    [HasPermission(Permissions.Invoices.Write)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var command = new DeleteInvoiceCommand(id);
        var result = await _sender.Send(command, cancellationToken);
        return result ? NoContent() : NotFound();
    }

    [HttpPost]
    [HasPermission(Permissions.Invoices.Write)]
    public async Task<IActionResult> Create(CreateInvoiceCommand command, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    [HasPermission(Permissions.Invoices.Write)]
    public async Task<IActionResult> Update(int id, UpdateInvoiceCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Invoice ID mismatch between request URL and body.");

        var result = await _sender.Send(command, cancellationToken);
        return result ? NoContent() : NotFound();
    }

    [HttpGet("{id:int}/pdf")]
    [HasPermission(Permissions.Invoices.Read)]
    public async Task<IActionResult> GetPdf(int id, CancellationToken cancellationToken)
    {
        var query = new GetInvoicePdfQuery(id);
        var pdfBytes = await _sender.Send(query, cancellationToken);
        return File(pdfBytes, "application/pdf", $"invoice_{id}.pdf");
    }
}
