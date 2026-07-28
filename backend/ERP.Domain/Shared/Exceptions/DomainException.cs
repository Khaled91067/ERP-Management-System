
namespace ERP.Domain.Shared.Exceptions;

public abstract class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}

