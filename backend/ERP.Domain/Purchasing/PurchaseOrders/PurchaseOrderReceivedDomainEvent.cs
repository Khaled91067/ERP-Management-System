
namespace ERP.Domain.Purchasing.PurchaseOrders;

using ERP.Domain.Shared.Abstractions;

public sealed record PurchaseOrderReceivedDomainEvent(int PurchaseOrderId) : IDomainEvent;