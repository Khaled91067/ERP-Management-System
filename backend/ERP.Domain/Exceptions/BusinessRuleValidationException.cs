namespace ERP.Domain.Exceptions;

public class BusinessRuleValidationException : DomainException
{
    public BusinessRuleValidationException(string message) : base(message)
    {
    }
}
