
namespace ERP.Application.Features.Authentication.Commands.Models
{
    using MediatR;

    public sealed record LogoutCommand(string RefreshToken) : IRequest;
}
