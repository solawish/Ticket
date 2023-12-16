using MediatR;

namespace Ticket.Application.Queries.TicketPlus.GetProductConfig;

public class GetProductConfigQuery : IRequest<GetProductConfigDto>
{
    /// <summary>
    /// Product Id
    /// </summary>
    /// <example>["p000002989"]</example>
    public IEnumerable<string> ProductId { get; set; }
}