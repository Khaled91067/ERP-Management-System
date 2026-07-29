
namespace ERP.Application.Features.Finance.Queries;

using MediatR;

public sealed record GetInvoicePdfQuery(int Id) : IRequest<byte[]>;
