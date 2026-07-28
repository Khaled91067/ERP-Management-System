namespace ERP.Domain.Shared.Base;

using System;
using ERP.Domain.Shared.Abstractions;

public abstract class AuditableEntity : BaseEntity, IAuditable
{
    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? LastModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }
}
