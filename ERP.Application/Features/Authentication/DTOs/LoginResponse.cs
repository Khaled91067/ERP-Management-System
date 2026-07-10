using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Application.Features.Authentication.DTOs;

public sealed record LoginResponse(string AccessToken);