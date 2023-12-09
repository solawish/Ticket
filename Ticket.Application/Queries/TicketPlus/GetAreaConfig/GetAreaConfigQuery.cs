using MediatR;

namespace Ticket.Application.Queries.TicketPlus.GetAreaConfig;

public class GetAreaConfigQuery : IRequest<GetAreaConfigDto>
{
    public IEnumerable<string> TicketAreaId { get; set; }
}