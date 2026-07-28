namespace ERP.Domain.Shared.Base;

using System;
using ERP.Domain.Shared.Abstractions;

public abstract class SoftDeletableEntity : AuditableEntity, ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
