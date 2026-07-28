
namespace ERP.Domain.Purchasing.PurchaseOrders;

using ERP.Domain.Shared.Base;
using ERP.Domain.Shared.Abstractions;

public sealed record PurchaseOrderReceivedDomainEvent(int PurchaseOrderId) : IDomainEvent;