using ERP.Domain.Common;

namespace ERP.Domain.Events;

public sealed record PurchaseOrderReceivedDomainEvent(int PurchaseOrderId) : IDomainEvent;