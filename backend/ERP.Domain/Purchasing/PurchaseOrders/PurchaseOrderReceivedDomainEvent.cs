
namespace ERP.Domain.Purchasing.PurchaseOrders;

using ERP.Domain.Shared.Common;

public sealed record PurchaseOrderReceivedDomainEvent(int PurchaseOrderId) : IDomainEvent;