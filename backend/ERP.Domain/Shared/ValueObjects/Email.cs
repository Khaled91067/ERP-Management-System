namespace ERP.Domain.Shared.ValueObjects;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ERP.Domain.Shared.Base;
using ERP.Domain.Shared.Exceptions;

public class Email : ValueObject
{
    private static readonly Regex EmailRegex = new Regex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new BusinessRuleValidationException("Email is required.");

        value = value.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(value))
            throw new BusinessRuleValidationException("Email format is invalid.");

        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
