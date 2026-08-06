namespace ERP.Domain.Shared.ValueObjects;

using System;
using System.Collections.Generic;

using ERP.Domain.Shared.Base;
using ERP.Domain.Shared.Exceptions;

public class Money : ValueObject
{
    public decimal Amount { get; }

    public Money(decimal amount)
    {
        if (amount < 0)
            throw new BusinessRuleValidationException("Money amount cannot be negative.");

        Amount = amount;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
    }

    public override string ToString() => Amount.ToString("G");

    public static Money operator +(Money left, Money right)
    {
        if (left == null || right == null)
            throw new ArgumentNullException("Cannot add null money.");
            
        return new Money(left.Amount + right.Amount);
    }

    public static Money operator -(Money left, Money right)
    {
        if (left == null || right == null)
            throw new ArgumentNullException("Cannot subtract null money.");
            
        return new Money(left.Amount - right.Amount);
    }

    public static Money operator *(Money left, decimal multiplier)
    {
        if (left == null)
            throw new ArgumentNullException("Cannot multiply null money.");
            
        return new Money(left.Amount * multiplier);
    }

    public static Money operator *(decimal multiplier, Money right)
    {
        if (right == null)
            throw new ArgumentNullException("Cannot multiply null money.");
            
        return new Money(multiplier * right.Amount);
    }
}
