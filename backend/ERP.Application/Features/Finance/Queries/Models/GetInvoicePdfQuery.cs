
namespace ERP.Application.Features.Finance.Queries.Models;

using MediatR;

public sealed record GetInvoicePdfQuery(int Id) : IRequest<byte[]>;
