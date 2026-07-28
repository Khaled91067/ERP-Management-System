
namespace ERP.Application.Features.Catalog.Commands;

using MediatR;

public sealed record AdjustStockCommand(int ProductId, int QuantityChange) : IRequest<bool>;
