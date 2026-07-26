namespace ERP.Application.Common.Exceptions;

public sealed class UnauthorizedException : Exception
{
    public UnauthorizedException(): base("Unauthorized.")
    {
    }

    public UnauthorizedException(string message): base(message)
    {
    }
}