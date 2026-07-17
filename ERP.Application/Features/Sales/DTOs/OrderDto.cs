namespace ERP.Application.Features.Sales.Dtos;

public sealed record OrderDto(
    int Id,
    int CustomerId,
    string CustomerName,
    DateTime OrderDate,
    string Status,
    string PaymentMethod,
    string ShippingAddress,
    decimal TotalAmount,
    List<OrderLineDto> Lines);

public sealed record OrderLineDto(
    int Id,
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal DiscountPercentage,
    decimal TotalPrice);
