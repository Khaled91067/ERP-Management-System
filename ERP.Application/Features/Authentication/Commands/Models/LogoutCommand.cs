using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Application.Features.Authentication.Commands.Models
{
    public sealed record LogoutCommand(string RefreshToken) : IRequest;
}
