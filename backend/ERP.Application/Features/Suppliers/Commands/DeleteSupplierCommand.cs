
namespace ERP.Application.Features.Suppliers.Commands;

using MediatR;

public sealed record DeleteSupplierCommand(int Id) : IRequest<bool>;
