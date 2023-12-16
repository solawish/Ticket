using MediatR;

namespace Ticket.Application.Queries.TicketPlus.GetAreaConfig;

public class GetAreaConfigQuery : IRequest<GetAreaConfigDto>
{
    /// <summary>
    /// Ticket Area Id
    /// </summary>
    /// <example>["a000000901"]</example>
    public IEnumerable<string> TicketAreaId { get; set; }
}