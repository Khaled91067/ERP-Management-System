using MediatR;

namespace ERP.Application.Features.Catalog.Commands;

public sealed record AdjustStockCommand(int ProductId, int QuantityChange) : IRequest<bool>;
