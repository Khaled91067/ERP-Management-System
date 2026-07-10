using ERP.Application.Features.Authentication.Commands.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserCommand command,CancellationToken cancellationToken)
    {
        var userId = await _sender.Send(
            command,
            cancellationToken);

        return Ok(new { id = userId });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command,CancellationToken cancellationToken)
    {
        var response = await _sender.Send(
            command,
            cancellationToken);

        return Ok(response);
    }

}