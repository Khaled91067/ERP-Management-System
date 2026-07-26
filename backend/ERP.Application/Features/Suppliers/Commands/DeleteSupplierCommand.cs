using MediatR;

namespace ERP.Application.Features.Suppliers.Commands;

public sealed record DeleteSupplierCommand(int Id) : IRequest<bool>;
